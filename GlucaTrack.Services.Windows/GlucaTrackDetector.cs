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
using GlucaTrack.Services.Windows;

namespace GlucaTrack.Services.Windows
{
    public partial class GlucaTrackDetector : ServiceBase
    {
        static string appId = "65aeed4c-956c-4873-ab4b-335b5e7f7835";
        BackgroundWorker background_DeviceDetector = null;
        BackgroundWorker background_DeviceReader = new BackgroundWorker();
        BackgroundWorker background_CommandServer = new BackgroundWorker();
        ManagementEventWatcher watcher = null;
        WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
        static Settings settings = null;

        public GlucaTrackDetector()
        {
            InitializeComponent();

            background_DeviceReader.WorkerSupportsCancellation = true;
            background_CommandServer.WorkerSupportsCancellation = true;

            pipeWrite("BUSYICON", "notbusy", string.Empty, 0);
        }

        protected override void OnStart(string[] args)
        {
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
            if (background_DeviceDetector != null)
            {
                background_DeviceDetector.CancelAsync();
                background_DeviceDetector.Dispose();
            }

            if (background_DeviceReader != null)
            {
                background_DeviceReader.CancelAsync();
                background_DeviceReader.Dispose();
            }

            if (background_CommandServer != null)
            {
                background_CommandServer.CancelAsync();
                background_CommandServer.Dispose();
            }

            watcher.EventArrived -= USBwatcher_EventArrived;
            watcher.Dispose();

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
            if (background_DeviceDetector != null) 
                return;

            //write entry to event log showing usb event arrived
            string deviceinfoMessage = "USB device inserted" + Environment.NewLine + Environment.NewLine;
            
            foreach (PropertyData pd in e.NewEvent.Properties)
            {
                ManagementBaseObject mbo = null;
                if ((mbo = pd.Value as ManagementBaseObject) != null)
                {
                    foreach (PropertyData prop in mbo.Properties)
                        deviceinfoMessage += string.Format("{0} - {1}" + Environment.NewLine, prop.Name, prop.Value);
                }
            }

            EventLog.WriteEntry(deviceinfoMessage, EventLogEntryType.Information);

            //start the background thread looking for devices
            startDetectorThread();
        }

        #region Thread Work
        private void background_DeviceDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "DeviceDetector";

            //change notify icon to busy
            pipeWrite("BUSYICON", "busy", string.Empty, 0);

            try
            {
                Common.Statics.deviceFound = null;
                DeviceInfo fdi = Communication.Statics.DetectFirstDevice();
                Common.Statics.deviceFound = fdi;
                e.Result = fdi;
            }
            finally
            {
                //change notify icon to busy
                pipeWrite("BUSYICON", "notbusy", string.Empty, 0);
            }
        }
        private void background_DeviceReader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "DeviceReader";

                //change notify icon to busy
                pipeWrite("BUSYICON", "busy", string.Empty, 0);

                //read devices data
                if (Common.Statics.deviceFound != null)
                {
                    if (typeof(IMeter).IsAssignableFrom(Common.Statics.deviceFound.DeviceType))
                    {
                        //serial devices
                        IMeter Meter = (IMeter)Common.Statics.deviceFound.Device;
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

                        if (Meter.IsPortOpen)
                        {
                            Meter.Port.DiscardInBuffer();
                            Meter.Port.DiscardOutBuffer();

                            Meter.ReadData();
                        }
                    }
                    else if (typeof(IMeterHID).IsAssignableFrom(Common.Statics.deviceFound.DeviceType))
                    {
                        //HID devices
                        IMeterHID Meter = (IMeterHID)Common.Statics.deviceFound.Device;
                        Meter.ReadFinished += new EventHandler(OnReadFinished);
                        Meter.RecordRead += new EventHandler(OnRecordRead);
                        Meter.HeaderRead += new EventHandler(OnHeaderRead);

                        EventLog.WriteEntry(string.Format("Begin reading data from {0}.", Common.Statics.deviceFound.DeviceDescription), EventLogEntryType.Information);

                        Meter.ReadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
                GlucaTrackDetector.ReportException("W0002", ex);
            }
        }
        private void background_CommandServer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "CommandServer";

            NamedPipeServerStream pipeServer = null;
            try
            {
                PipeSecurity pipeSecurity = new PipeSecurity();
                pipeSecurity.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
                pipeServer = new NamedPipeServerStream("pipeGlucaTrackDetectorIn", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.None, 512, 512, pipeSecurity, HandleInheritability.None);
                //pipeServer = new NamedPipeServerStream("pipeGlucaTrackDetectorIn", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

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
                    case "CHECK_FOR_UPDATES":
                        pipeWrite("UPDATE_CHECK_FINISHED", CheckForUpdates(), String.Empty, 1);
                        break;
                    case "REPORT_BUG":

                        break;
                    default: break;
                };
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
                GlucaTrackDetector.ReportException("W0003", ex);
            }
            finally
            {
                if (pipeServer != null)
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
                GlucaTrackDetector.ReportException("W0004", ex);
            }
        }
        private void background_DeviceReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //finished reading - OnReadFinished raised

            //change notify icon to busy
            pipeWrite("BUSYICON", "notbusy", string.Empty, 0);
        }
        private void background_CommandServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            background_CommandServer.RunWorkerAsync();
        }
        #endregion

        private static void pipeWrite(string command)
        {
            pipeWrite(command, string.Empty, string.Empty, 1);
        }
        private static void pipeWrite(string command, string Text1, string Text2, int icon)
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
                Exception ex = new Exception("Notify icon app not started so no settings file could be read.  Aborting read.");
                Errors.ServiceError(ex);
                GlucaTrackDetector.ReportException("W0005", ex);
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
                GlucaTrackDetector.ReportException("W0006", ex);
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
        
        private void uploadData(object oMeter)
        {
            if (settings != null)
            {
                string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
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
                            userinfo = client.ValidateLogin(StringCipher.Encrypt(assemblyName), StringCipher.Encrypt(appId), 
                                                            StringCipher.Encrypt(loginRow.Username), StringCipher.Encrypt(loginRow.Password));
                        }
                        catch (Exception ex)
                        {
                            //could not validate user
                            Errors.ServiceError(ex);
                            GlucaTrackDetector.ReportException("W0007", ex);
                        }
                        finally 
                        {
                            EventLog.WriteEntry("Validating User: End", EventLogEntryType.Information);
                        }

                        try
                        {
                            WebService.Records recordsToUpload = new WebService.Records();

                            foreach (Records.RecordRow row in ((AbstractMeter)oMeter).Records)
                            {
                                ((WebService.Records.RecordDataTable)(recordsToUpload.Tables[0])).AddRecordRow(row.Timestamp, row.Glucose, row.Units);
                            }
                            
                            EventLog.WriteEntry("Uploading Data: Begin", EventLogEntryType.Information);

                            client.PostGlucoseRecords(recordsToUpload, userinfo, ((AbstractMeter)oMeter).ID);

                            EventLog.WriteEntry("Uploading Data: End", EventLogEntryType.Information);

                            pipeWrite("MSG", "Sent Glucose Readings", "to GlucaTrack website", 1);
                        }
                        catch(Exception ex)
                        {
                            Errors.ServiceError(ex);
                            GlucaTrackDetector.ReportException("W0008", ex);
                        }
                    }
                    else
                    {
                        Exception ex = new Exception("uploadData-2: Could not upload data due to settings not being populated.");
                        Errors.ServiceError(ex);
                        GlucaTrackDetector.ReportException("W0009", ex);
                    }
                }
            }
            else
            {
                Exception ex = new Exception("uploadData-1: Could not upload data due to settings not being populated.");
                Errors.ServiceError(ex);
                GlucaTrackDetector.ReportException("W000A", ex);
            }
        }
        private string CheckForUpdates()
        {
            try
            {
                using (WebService.GTServiceClient client = new WebService.GTServiceClient())
                {
                    string updatePath = client.IsUpdatePresent(appId, Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    return updatePath;
                }
            }
            catch { return string.Empty; }
        }

        protected virtual void OnReadFinished(object sender, EventArgs e)
        {
            object meter = ((ReadFinishedEventArgs)e).Meter;

            try
            {
                if (meter == null)
                    throw new Exception("OnReadFinished-1: Meter object was null.");

                EventLog.WriteEntry(string.Format("Finished reading {1} data records from {0}.", ((AbstractMeter)meter).MeterDescription, ((AbstractMeter)meter).Records.Count), EventLogEntryType.Information);

                pipeWrite("MSG", string.Format("Finished reading {0} data records", ((AbstractMeter)meter).Records.Count), string.Format("from {0}", ((AbstractMeter)meter).MeterDescription), 1);

                //send data to webservice
                uploadData(meter);
            }
            catch (Exception ex)
            {
                Errors.ServiceError(ex);
                GlucaTrackDetector.ReportException("W000B", ex);
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
            
            pipeWrite("MSG", string.Format("Reading Glucose Values"), string.Format("from {0}", ((AbstractMeter)headArgs.Meter).MeterDescription), 1);
        }

        public static void ReportException(string ErrorCode, Exception ex)
        {
            try
            {
                if (settings == null || settings.Options == null || settings.Options.Rows.Count == 0 || !Convert.ToBoolean(settings.Options.Rows[0]["AutoReportErrors"]))
                    return;

                using (WebService.GTServiceClient client = new WebService.GTServiceClient())
                {
                    client.ReportBug(appId, ErrorCode, ex.StackTrace ?? string.Empty, ex.Message ?? string.Empty, Assembly.GetCallingAssembly().GetName().Version.ToString());
                }
            }
            catch(Exception ex2)
            {
                Errors.ServiceError(ex2);
            }
        }
    }
}
