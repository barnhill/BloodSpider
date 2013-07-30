using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Xml.Serialization;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Services.Windows
{
    public partial class formSettings : Form
    {
        //TODO: move all strings to resource file
        //TODO: add verbose output option for popups
        BackgroundWorker background_CommandServer = new BackgroundWorker();

        public formSettings()
        {
            InitializeComponent();

            //make settings window not visible
            ShowSettings(false);

            //start named pipe communication thread
            background_CommandServer.WorkerSupportsCancellation = true;
            background_CommandServer.DoWork += background_CommandServer_DoWork;
            background_CommandServer.RunWorkerCompleted += background_CommandServer_RunWorkerCompleted;
            background_CommandServer.RunWorkerAsync();

            //gets the version from the service and displays it on the right click menu
            GetVersionFromService();
        }

        #region Threads
        private void background_CommandServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            background_CommandServer.RunWorkerAsync();
        }
        private void background_CommandServer_DoWork(object sender, DoWorkEventArgs e)
        {
            NamedPipeServerStream pipeServer = null;
            try
            {
                pipeServer = new NamedPipeServerStream("pipeGlucaTrackDetectorOut", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                pipeServer.WaitForConnection();

                StreamString ss = new StreamString(pipeServer);

                //verify identity
                ss.WriteString("GlucaTrack_NotifyIcon");

                //get command from client
                var command = ss.ReadString();
                var split = command.Split(new string[] { "|" }, StringSplitOptions.None);

                ToolTipIcon icon = ToolTipIcon.None;
                switch (split[3])
                {
                    case "1": icon = ToolTipIcon.Info;
                        break;
                    case "2": icon = ToolTipIcon.Warning;
                        break;
                    case "3": icon = ToolTipIcon.Error;
                        break;
                    default: break;
                }

                switch (split[0].ToUpperInvariant().Trim())
                {
                    case "MSG":
                        notifyIcon1.ShowBalloonTip(2500, split[1], split[2], icon);
                        break;
                    case "ULD":
                        DialogResult result = MessageBox.Show(split[2], split[1], MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Yes)
                        {
                            pipeWrite("ULD_YES", string.Empty, string.Empty, 0);
                        }
                        break;
                    case "PATH_REQ":
                        pipeWrite("ULD_PATH_RESP", Path.Combine(Common.Statics.baseFilepath, "glucatrack.sav"), string.Empty, 0);
                        break;
                    default: break;
                };
            }
            catch (Exception)
            {
            }
            finally 
            {
                pipeServer.Close();
            }
        }
        #endregion

        #region UI Interaction
        private void menuItem_Settings_Click(object sender, EventArgs e)
        {
            PopulateScreenFromSettings(Statics.ReadSettingsFile());
            ShowSettings(true);
        }
        private void menuItem_Website_Click(object sender, EventArgs e)
        {
            ShowWebsite();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShowSettings(false);
        }
        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            background_CommandServer.Dispose();
            Application.Exit();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //save settings on save button click
            try
            {
                using (Settings settings = new Settings())
                {
                    Settings.LoginRow loginRow = settings.Login.NewLoginRow();
                    loginRow.Username = StringCipher.Encrypt(this.txtUsername.Text.Trim(), true);
                    loginRow.Password = StringCipher.Encrypt(this.txtPassword.Text.Trim(), true);

                    settings.Login.AddLoginRow(loginRow);

                    settings.Options.AddOptionsRow(!chkAutoUpload.Checked);

                    //save settings file
                    Statics.SaveSettingsFile(settings);

                    //show that all settings were saved successfully
                    notifyIcon1.ShowBalloonTip(2500, "GlucaTrack Settings", "User settings were successfully saved.", ToolTipIcon.Info);
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                Errors.Error(uaex);
            }
            catch (Exception ex)
            {
                Errors.Error(ex);
            }
            finally
            {
                ShowSettings(false);
            }
        }
        #endregion

        private void GetVersionFromService()
        {
            NamedPipeClientStream pipeServerIn = null;
            try
            {
                pipeServerIn = new NamedPipeClientStream(".", "pipeGlucaTrackDetectorIn");
                if (!pipeServerIn.IsConnected)
                    pipeServerIn.Connect(2000);

                StreamString ss = new StreamString(pipeServerIn);

                //verify server identity
                if (ss.ReadString() == "GlucaTrack_Service")
                {
                    //talking to the correct service so send the message
                    ss.WriteString("VERSION|");
                    menuItem_Version.Text = "GlucaTrack " + ss.ReadString();
                    notifyIcon1.Text = menuItem_Version.Text;
                    tStartService.Stop();
                }
            }//try
            catch (System.TimeoutException)
            {
                ServiceController service = new ServiceController(Statics.serviceName);
                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(2000);

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    tStartService.Stop();
                    GetVersionFromService();
                }
                catch(Exception ex)
                {
                    if (ex.Message.ToLowerInvariant().Contains("was not found on computer"))
                    {
                        //service not installed on computer so stop looking for service and display message
                        tStartService.Stop();
                        MessageBox.Show(Statics.serviceName + " service not found on this computer.  Please reinstall.", Statics.serviceName + " service not found", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        tStartService.Start();
                    }
                }
            }
            finally
            {
                pipeServerIn.Close();
            }
        }
        private void pipeWrite(string command, string Text1, string Text2, int icon)
        {
            //icon = (1 = Info, 2 = Warning, 3 = Error)
            NamedPipeClientStream pipeServerOut = null;
            try
            {
                pipeServerOut = new NamedPipeClientStream(".", "pipeGlucaTrackDetectorIn");
                if (!pipeServerOut.IsConnected)
                    pipeServerOut.Connect(2000);

                StreamString ss = new StreamString(pipeServerOut);

                //verify server identity
                if (ss.ReadString() == "GlucaTrack_Service")
                {
                    //talking to the correct service so send the message
                    ss.WriteString(command + "|" + Text1 + "|" + Text2 + "|" + (icon == 0 ? "" : icon.ToString()));
                }
            }//try
            catch (System.TimeoutException tex)
            {
                Errors.Error(tex);
            }
            catch (Exception ex)
            {
                Errors.Error(ex);
            }
            finally
            {
                pipeServerOut.Close();
            }
        }
        private void ShowSettings(bool show)
        {
            this.Visible = show;

            if (show)
            {
                //move settings window to bottom right corner of screen
                this.Size = new Size(200, 192);
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                PopulateScreenFromSettings(Statics.ReadSettingsFile());
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }
        private void ShowWebsite()
        {
            Process.Start("http://www.glucatrack.com");
        }
        private void PopulateScreenFromSettings(Settings settings)
        {
            if (settings != null && settings.Login != null)
            {
                Settings.LoginRow loginRow = settings.Login.FirstOrDefault();

                if (loginRow != null)
                {
                    this.txtUsername.Text = StringCipher.Decrypt(loginRow.Username, true);
                    this.txtPassword.Text = StringCipher.Decrypt(loginRow.Password, true);
                }
            }

            if (settings != null && settings.Options != null)
            {
                Settings.OptionsRow optionRow = settings.Options.FirstOrDefault();

                if (optionRow != null)
                {
                    this.chkAutoUpload.Checked = !optionRow.AutoUpload;
                }
            }
        }

        private void tStartService_Tick(object sender, EventArgs e)
        {
            ServiceController service = new ServiceController(Statics.serviceName);
            try
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(2000));
                GetVersionFromService();
            }
            catch(Exception ex)
            {
                if (ex.Message.ToLowerInvariant().Contains("was not found on computer"))
                {
                    //service not installed on computer so stop looking for service and display message
                    tStartService.Stop();
                    MessageBox.Show(Statics.serviceName + " service not found on this computer.  Please reinstall.", Statics.serviceName + " service not found", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }
    }
}
