using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Management;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using GlucaTrack.Communication;
using GlucaTrack.Communication.Meters;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Services.Windows
{
    public partial class GlucaTrackDetector : ServiceBase
    {
        BackgroundWorker background_DeviceDetector = null;
        BackgroundWorker background_DeviceReader = new BackgroundWorker();
        BackgroundWorker background_CommandServer = new BackgroundWorker();
        ManagementEventWatcher watcher = null;
        WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
        Settings settings = null;

        public GlucaTrackDetector()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //if (!Debugger.IsAttached)
            //    Debugger.Launch();

            watcher = new ManagementEventWatcher();
            watcher.EventArrived += USBwatcher_EventArrived;
            watcher.Query = query;
            watcher.Start();

            //starts the background pipe command server
            startPipeServerThread();

            EventLog.WriteEntry("The service was started successfully.", EventLogEntryType.Information);
        }
        protected override void OnStop()
        {
            background_DeviceDetector.CancelAsync();
            watcher.EventArrived -= USBwatcher_EventArrived;
            watcher.Dispose();

            background_CommandServer.CancelAsync();
            background_DeviceReader.CancelAsync();
            background_DeviceDetector.CancelAsync();

            EventLog.WriteEntry("The service was stopped successfully.", EventLogEntryType.Information);
        }
        protected override void OnShutdown()
        {
            background_DeviceDetector.CancelAsync();
            watcher.EventArrived -= USBwatcher_EventArrived;
            watcher.Dispose();

            EventLog.WriteEntry("The service was shutdown successfully", EventLogEntryType.Information);
        }

        private void USBwatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (background_DeviceDetector != null) return;
            
            //start the background thread looking for devices
            startDetectorThread();
        }

        #region Thread Work
        private void background_DeviceDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            Common.Statics.deviceFound = null;
            DeviceInfo fdi = Communication.Statics.DetectFirstDevice();
            Common.Statics.deviceFound = fdi;
            e.Result = fdi;
        }
        private void background_DeviceReader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //read devices data
                if (Common.Statics.deviceFound != null)
                {
                    IMeter Meter = (IMeter)Activator.CreateInstance(Common.Statics.deviceFound.DeviceType);
                    Meter.ReadFinished += new EventHandler(OnReadFinished);
                    Meter.RecordRead += new EventHandler(OnRecordRead);
                    Meter.HeaderRead += new EventHandler(OnHeaderRead);

                    int connectionTries = 0;
                    while (!Meter.Connect(Common.Statics.deviceFound.ComPortName) && connectionTries < 30)
                    {
                        Thread.Sleep(100);
                        connectionTries++;
                    }

                    EventLog.WriteEntry(string.Format("Begin reading data from {0}.", Common.Statics.deviceFound.DeviceDescription), EventLogEntryType.Information);

                    Meter.Port.DiscardInBuffer();
                    Meter.Port.DiscardOutBuffer();

                    Meter.ReadData();
                }
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
            }
        }
        private void background_CommandServer_DoWork(object sender, DoWorkEventArgs e)
        {
            NamedPipeServerStream pipeServer = null;
            try
            {
                pipeServer = new NamedPipeServerStream("pipeGlucaTrackDetectorIn", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                pipeServer.WaitForConnection();

                StreamString ss = new StreamString(pipeServer);

                //verify identity
                ss.WriteString("GlucaTrack_Service");

                //get command from client
                var command = ss.ReadString();
                var split = command.Split(new string[] { "|" }, StringSplitOptions.None);

                switch (split[0].ToUpperInvariant().Trim())
                {
                    case "VERSION":
                        Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                        ss.WriteString(ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString());
                        break;
                    case "ULD_YES":
                        startReaderThread();
                        break;
                    case "ULD_PATH_RESP":
                        settings = Common.Statics.ReadSettingsFile(split[1]);
                        
                        if (settings != null && settings.Options != null)
                        {
                            if (settings.Options.Rows.Count > 0 && (bool)settings.Options.Rows[0]["AutoUpload"])
                            {
                                //auto upload
                                startReaderThread();
                            }
                            else
                            {
                                //no auto upload, require user input
                                if (Common.Statics.deviceFound != null)
                                {
                                    pipeWrite("ULD", "Found Meter", "Would you like to upload data from the " + Common.Statics.deviceFound.DeviceDescription + "?", 1);
                                }
                            }
                        }
                        break;
                    default: break;
                };
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
            }
            finally
            {
                pipeServer.Close();
            }
        }
        #endregion

        #region Thread Finished
        private void background_DeviceDetector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                background_DeviceDetector.Dispose();
                background_DeviceDetector = null;

                if (e != null && e.Result != null && Common.Statics.deviceFound != null)
                {
                    EventLog.WriteEntry("Device identified: " + Common.Statics.deviceFound.DeviceDescription, EventLogEntryType.Information, 1);
                    
                    //send command to show message balloon on notify icon
                    pipeWrite("MSG", "Found Meter", Common.Statics.deviceFound.DeviceDescription, 1);

                    //request the path to the settings file
                    pipeWrite("PATH_REQ");
                }
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
            }
        }
        private void background_DeviceReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //finished reading - OnReadFinished raised
        }
        private void background_CommandServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            background_CommandServer.RunWorkerAsync();
        }
        #endregion

        private void pipeWrite(string command)
        {
            pipeWrite(command, string.Empty, string.Empty, 1);
        }
        private void pipeWrite(string command, string Text1, string Text2, int icon)
        {
            //icon = (1 = Info, 2 = Warning, 3 = Error)
            NamedPipeClientStream pipeServerOut = null;
            try
            {
                pipeServerOut = new NamedPipeClientStream(".", "pipeGlucaTrackDetectorOut");
                if (!pipeServerOut.IsConnected)
                    pipeServerOut.Connect(2000);

                StreamString ss = new StreamString(pipeServerOut);

                //verify server identity
                if (ss.ReadString() == "GlucaTrack_NotifyIcon")
                {
                    //talking to the correct service so send the message
                    ss.WriteString(command + "|" + Text1 + "|" + Text2 + "|" + (icon == 0 ? "" : icon.ToString()));
                }
            }//try
            catch (System.TimeoutException)
            {
                Errors.ServiceError(new Exception("Notify icon app not started so no settings file could be read.  Aborting read."));
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
            }
            finally 
            {
                pipeServerOut.Close();
            }
        }
        private void startPipeServerThread()
        {
            if (background_CommandServer != null)
            {
                background_CommandServer.Dispose();
            }

            background_CommandServer = new BackgroundWorker();
            background_CommandServer.WorkerSupportsCancellation = true;
            background_CommandServer.DoWork += background_CommandServer_DoWork;
            background_CommandServer.RunWorkerCompleted += background_CommandServer_RunWorkerCompleted;
            background_CommandServer.RunWorkerAsync();
        }
        private void startDetectorThread()
        {
            if (background_DeviceDetector != null)
            {
                background_DeviceDetector.Dispose();
                background_DeviceDetector = null;
            }

            background_DeviceDetector = new BackgroundWorker();
            background_DeviceDetector.WorkerSupportsCancellation = true;
            background_DeviceDetector.DoWork += background_DeviceDetector_DoWork;
            background_DeviceDetector.RunWorkerCompleted += background_DeviceDetector_RunWorkerCompleted;
            background_DeviceDetector.RunWorkerAsync();
        }
        private void startReaderThread()
        {
            if (background_DeviceReader != null)
            {
                background_DeviceReader.Dispose();
                background_DeviceReader = null;
            }

            background_DeviceReader = new BackgroundWorker();
            background_DeviceReader.WorkerSupportsCancellation = true;
            background_DeviceReader.DoWork += background_DeviceReader_DoWork;
            background_DeviceReader.RunWorkerCompleted += background_DeviceReader_RunWorkerCompleted;
            background_DeviceReader.RunWorkerAsync();
        }
        private void uploadData(IMeter meter)
        {
            if (settings != null)
            {
                using (WebService.GTServiceClient client = new WebService.GTServiceClient())
                {
                    //upload meter info to database via web service
                    Settings.LoginRow loginRow = settings.Login.FirstOrDefault();
                    if (loginRow != null && !loginRow.IsUsernameNull() && !loginRow.IsPasswordNull())
                    {
                        WebService.Common userinfo = null;
                        try
                        {
                            EventLog.WriteEntry("Validating User: Begin", EventLogEntryType.Information);

                            //validate user
                            userinfo = client.ValidateLogin(Common.StringCipher.DES_Decrypt(loginRow.Username), loginRow.Password);
                        }
                        catch (Exception ex)
                        {
                            //could not validate user
                            Errors.ServiceError(ex);
                        }
                        finally 
                        {
                            EventLog.WriteEntry("Validating User: End", EventLogEntryType.Information);
                        }

                        try
                        {
                            WebService.Records recordsToUpload = new WebService.Records();

                            foreach (Records.RecordRow row in meter.Records)
                            {
                                ((WebService.Records.RecordDataTable)(recordsToUpload.Tables[0])).AddRecordRow(row.Timestamp, row.Glucose, row.Units);
                            }

                            EventLog.WriteEntry("Uploading Data: Begin", EventLogEntryType.Information);

                            client.PostGlucoseRecords(recordsToUpload, userinfo, meter.ID);
                            client.UpdateLastSync(userinfo);

                            EventLog.WriteEntry("Uploading Data: End", EventLogEntryType.Information);
                        }
                        catch(Exception ex)
                        {
                            Errors.ServiceError(ex);
                        }
                    }
                    else
                    {
                        Errors.ServiceError(new Exception("uploadData-2: Could not upload data due to settings not being populated."));
                    }
                }
            }
            else
            {
                Errors.ServiceError(new Exception("uploadData-1: Could not upload data due to settings not being populated."));
            }
        }

        protected virtual void OnReadFinished(object sender, EventArgs e)
        {
            IMeter meter = ((ReadFinishedEventArgs)e).Meter;

            try
            {
                EventLog.WriteEntry(string.Format("Finished reading data from {0}.", Common.Statics.deviceFound.DeviceDescription), EventLogEntryType.Information);

                if (meter == null)
                    throw new Exception("OnReadFinished-1: Meter object was null.");

                //send data to webservice
                uploadData(meter);
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
            }
        }
        protected virtual void OnRecordRead(object sender, EventArgs e)
        {
            RecordReadEventArgs readArgs = (RecordReadEventArgs)e;

            //TODO: put progress bar on reading code here
        }
        
        protected virtual void OnHeaderRead(object sender, EventArgs e)
        {
            HeaderReadEventArgs headArgs = (HeaderReadEventArgs)e;
            
            pipeWrite("MSG", string.Format("Reading {0} Glucose Values", headArgs.RowCount), string.Format("from {0}", headArgs.Meter.MeterDescription), 1);
        }
    }
}
