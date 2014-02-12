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
using System.Reflection;
using System.Resources;
using System.ServiceProcess;
using System.Threading;
using System.Xml.Serialization;
using BloodSpider.Services.Common;
using BloodSpider.Services.Windows.NotifyIcon.Properties;

namespace BloodSpider.Services.Windows
{
    public partial class formSettings : Form
    {
        enum Icons : int { Enabled = 0, Disabled = 1, Busy = 2 }

        //TODO: move all strings to resource file
        BackgroundWorker background_CommandServer = new BackgroundWorker();
        static BloodSpider.Services.Common.Settings _settings;
        string currentVersion = string.Empty;
        Size _windowSize = new Size(193, 262);
        bool busy;
        int state = 0;

        public formSettings()
        {
            InitializeComponent();

            ChangeNotifyIcon(Icons.Disabled);

            //gets the version from the service and displays it on the right click menu
            GetVersionFromService();

            if (!CheckSettingsFile())
                ShowWindow(true, true);
            else
                ShowWindow(false);

            //start named pipe communication thread
            background_CommandServer.WorkerSupportsCancellation = true;
            background_CommandServer.DoWork += background_CommandServer_DoWork;
            background_CommandServer.RunWorkerCompleted += background_CommandServer_RunWorkerCompleted;
            background_CommandServer.RunWorkerAsync();
        }

        #region Threads
        private void background_CommandServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            background_CommandServer.RunWorkerAsync();
        }
        private void background_CommandServer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "CommandServer";

            NamedPipeServerStream pipeServer = null;
            try
            {
                pipeServer = new NamedPipeServerStream("pipeBloodSpiderDetectorOut", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                pipeServer.WaitForConnection();

                StreamString ss = new StreamString(pipeServer);

                //verify identity
                ss.WriteString("BloodSpider_NotifyIcon");

                //get command from client
                var command = ss.ReadString();
                var split = command.Split(new string[] { "|" }, StringSplitOptions.None);

                switch (split[0].ToUpperInvariant().Trim())
                {
                    case "MSG":
                        ToolTipIcon icon = ToolTipIcon.None;
                        switch (split[3])
                        {
                            case "1": icon = ToolTipIcon.Info;
                                break;
                            case "2": icon = ToolTipIcon.Warning;
                                break;
                            case "3": icon = ToolTipIcon.Error;
                                break;
                            default:
                                break;
                        }
                        ShowNotificationBalloon(5000, split[1], split[2], icon);
                        break;
                    case "ULD":
                        DialogResult result = MessageBox.Show(split[2], split[1], MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Yes)
                        {
                            pipeWrite("ULD_YES", string.Empty, string.Empty, 0);
                        }
                        break;
                    case "BUSYICON":
                        switch (split[1].ToLowerInvariant())
                        {
                            case "busy": ChangeNotifyIcon(Icons.Busy);
                                break;
                            case "notbusy": ChangeNotifyIcon(Icons.Enabled);
                                break;
                            case "disabled": ChangeNotifyIcon(Icons.Disabled);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "PATH_REQ":
                        pipeWrite("ULD_PATH_RESP", Path.Combine(Common.Statics.BaseFilepath, "BloodSpider.sav"), string.Empty, 0);
                        break;
                    case "UPDATE_CHECK_FINISHED":
                        PerformUpdate(split[1]);
                        break;
                    default: break;
                };
            }
            catch (Exception)
            {
            }
            finally
            {
                if (pipeServer != null)
                    pipeServer.Close();
            }
        }
        private void background_BalloonTip_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            settingsNotifyIcon.Visible = false;
            settingsNotifyIcon.Visible = true;
            settingsNotifyIcon.BalloonTipClicked -= settingsNotifyIcon_BalloonTipClicked;
        }
        private void background_BalloonTip_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "BalloonTip";

            if (_settings != null 
                && _settings.Options != null 
                && _settings.Options.Rows.Count > 0 
                && !_settings.Options.FirstOrDefault().ShowNotifications)
                return;

            BalloonParameters bparms = (BalloonParameters)e.Argument;

            if (bparms.Timeout == 0)
                bparms.Timeout = int.MaxValue;

            if (bparms.Timeout < 0)
                return;

            if (bparms.Icon == ToolTipIcon.Warning || bparms.Icon == ToolTipIcon.Error)
                settingsNotifyIcon.BalloonTipClicked += settingsNotifyIcon_BalloonTipClicked;

            settingsNotifyIcon.ShowBalloonTip(bparms.Timeout, bparms.Title, bparms.Message, bparms.Icon);
 
            Thread.Sleep(bparms.Timeout);
        }
        #endregion

        #region UI Interaction
        private void menuItem_Settings_Click(object sender, EventArgs e)
        {
            PopulateScreenFromSettings(Statics.ReadSettingsFile());
            ShowWindow(true);
        }
        private void menuItem_Website_Click(object sender, EventArgs e)
        {
            ShowWebsite();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShowWindow(false);
        }
        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            background_CommandServer.Dispose();
            Application.Exit();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void settingsNotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            ShowWindow(true);
        }
        private void settingsNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow(true);
        }
        #endregion

        #region Timers
        private void tStartService_Tick(object sender, EventArgs e)
        {
            using (ServiceController service = new ServiceController(Statics.serviceName))
            {
                try
                {
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(2000));
                    }

                    GetVersionFromService();
                }
                catch (Exception)
                {
                    settingsNotifyIcon.Text = "BloodSpider service not running.";
                    ChangeNotifyIcon(Icons.Disabled);
                }
            }
        }
        private void tLookForUpdate_Tick(object sender, EventArgs e)
        {
            //reset the timer to be once per 24hrs and 1 second
            tLookForUpdate.Interval = 86401000;

            //check online for updates, if one is available start the install
            CheckForUpdates();
        }
        #endregion

        private static void Error(string ErrorCode, Exception ex)
        {
            //error in windows application
            ReportBug("WA" + ErrorCode, ex.StackTrace, ex.Message);
        }

        private void SaveSettings()
        {
            //save settings on save button click
            try
            {
                using (BloodSpider.Services.Common.Settings settings = new BloodSpider.Services.Common.Settings())
                {
                    BloodSpider.Services.Common.Settings.LoginRow loginRow = settings.Login.NewLoginRow();
                    loginRow.Username = StringCipher.Encrypt(this.txtUsername.Text.Trim(), true);
                    loginRow.Password = StringCipher.Encrypt(this.txtPassword.Text.Trim(), true);

                    settings.Login.AddLoginRow(loginRow);

                    settings.Options.AddOptionsRow(!chkAutoUpload.Checked, chkShowNotifications.Checked, chkAutoReportErrors.Checked);

                    _settings = settings;

                    //save settings file
                    Statics.SaveSettingsFile(_settings);

                    //show that all settings were saved successfully
                    ShowNotificationBalloon(3000, "BloodSpider Settings", "User settings were successfully saved.", ToolTipIcon.Info);
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                Error("002", uaex);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrWhiteSpace(this.txtUsername.Text))
                    ShowNotificationBalloon(10000, "BloodSpider Settings", "Username can not be blank.", ToolTipIcon.Error);
                else if (string.IsNullOrWhiteSpace(this.txtPassword.Text))
                    ShowNotificationBalloon(10000, "BloodSpider Settings", "Password can not be blank.", ToolTipIcon.Error);

                Error("003", ex);
            }
            finally
            {
                ShowWindow(false);
            }
        }
        private void GetVersionFromService()
        {
            NamedPipeClientStream pipeServerIn = null;
            try
            {
                pipeServerIn = new NamedPipeClientStream(".", "pipeBloodSpiderDetectorIn", PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Anonymous, HandleInheritability.Inheritable);
                if (!pipeServerIn.IsConnected)
                    pipeServerIn.Connect(2000);

                StreamString ss = new StreamString(pipeServerIn);

                //verify server identity
                if (ss.ReadString() == "BloodSpider_Service")
                {
                    //show the connected icon, if not busy
                    if (!busy)
                        ChangeNotifyIcon(Icons.Enabled);

                    //talking to the correct service so send the message
                    ss.WriteString("VERSION|");
                    currentVersion = ss.ReadString();
                    menuItem_Version.Text = "BloodSpider " + currentVersion;
                    settingsNotifyIcon.Text = menuItem_Version.Text;
                    this.Text = "BloodSpider Settings v" + currentVersion; 
                }
            }//try
            catch (System.TimeoutException)
            {
                ChangeNotifyIcon(Icons.Disabled);
            }
            finally
            {
                if (pipeServerIn != null)
                    pipeServerIn.Close();
            }
        }
        private void CheckForUpdates()
        {
            NamedPipeClientStream pipeServerIn = null;
            try
            {
                if (File.Exists(Statics.LocalUpdateFilePath))
                {
                    try
                    {
                        //remove the previously downloaded update
                        File.Delete(Statics.LocalUpdateFilePath);
                    }
                    catch { }
                }

                UpdateInfo.Update_InfoRow uir = Statics.ReadUpdateInfoFile();
                if (uir != null)
                {
                    DateTime? dtLastCheck = uir.LastUpdateCheck;
                    if (dtLastCheck != null && DateTime.Now - dtLastCheck < TimeSpan.FromDays(1))
                    {
                        return;
                    }
                }

                //save the last time the update check was run
                UpdateInfo uinfo = new UpdateInfo();
                uinfo.Update_Info.AddUpdate_InfoRow(DateTime.Now);
                Statics.SaveUpdateInfoFile(uinfo);

                pipeServerIn = new NamedPipeClientStream(".", "pipeBloodSpiderDetectorIn");
                if (!pipeServerIn.IsConnected)
                    pipeServerIn.Connect(2000);

                StreamString ss = new StreamString(pipeServerIn);

                //verify server identity
                if (ss.ReadString() == "BloodSpider_Service")
                {
                    //show the connected icon
                    ChangeNotifyIcon(Icons.Enabled);

                    //talking to the correct service so send the message
                    ss.WriteString("CHECK_FOR_UPDATES|");
                }
            }//try
            catch (Exception ex)
            { 
            }
            finally
            {
                if (pipeServerIn != null)
                    pipeServerIn.Close();
            }
        }
        private void PopulateScreenFromSettings(BloodSpider.Services.Common.Settings settings)
        {
            if (settings != null && settings.Login != null)
            {
                BloodSpider.Services.Common.Settings.LoginRow loginRow = settings.Login.FirstOrDefault();

                if (loginRow != null)
                {
                    this.txtUsername.Text = StringCipher.Decrypt(loginRow.Username, true);
                    this.txtPassword.Text = StringCipher.Decrypt(loginRow.Password, true);
                }
            }

            if (settings != null && settings.Options != null)
            {
                BloodSpider.Services.Common.Settings.OptionsRow optionRow = settings.Options.FirstOrDefault();

                if (optionRow != null)
                {
                    this.chkAutoUpload.Checked = !optionRow.AutoUpload;
                    this.chkShowNotifications.Checked = optionRow.ShowNotifications;
                    this.chkAutoReportErrors.Checked = optionRow.AutoReportErrors;
                }
            }
        }
        private void ShowNotificationBalloon(int timeout, string title, string message, ToolTipIcon icon)
        {
            BalloonParameters parms = new BalloonParameters(timeout, title, message, icon);

            BackgroundWorker background_BalloonTip = new BackgroundWorker();
            background_BalloonTip.WorkerSupportsCancellation = true;
            background_BalloonTip.DoWork += background_BalloonTip_DoWork;
            background_BalloonTip.RunWorkerCompleted += background_BalloonTip_RunWorkerCompleted;
            background_BalloonTip.RunWorkerAsync(parms);
        }
        private void ChangeNotifyIcon(Icons icon)
        {
            bool changed = false;
            switch (icon)
            {
                case Icons.Enabled:
                    if (state != 0)
                    {
                        settingsNotifyIcon.Icon = Resources.blood_enabled;
                        changed = true;
                        state = 0;
                    }
                    busy = false;
                    break;
                case Icons.Disabled:
                    if (state != 1)
                    {
                        settingsNotifyIcon.Icon = Resources.blood_disabled;
                        changed = true;
                        state = 1;
                    }
                    busy = false;
                    break;
                case Icons.Busy:
                    if (state != 2)
                    {
                        settingsNotifyIcon.Icon = Resources.blood_busy;
                        changed = true;
                        state = 2;
                    }
                    busy = true;
                    break;
                default:
                    break;
            }

            if (changed)
            {
                settingsNotifyIcon.Visible = false;
                settingsNotifyIcon.Visible = true;
            }
        }
        private void PerformUpdate(string webPath)
        {
            if (!string.IsNullOrWhiteSpace(webPath))
            {
                Windows.Update u = new Windows.Update(webPath);
            }
        }
        private void ShowWebsite()
        {
            try
            {
                Process.Start("http://www.BloodSpider.com");
            }
            catch (Exception ex)
            {
                Error("005", ex);
            }
        }
        private void ShowWindow(bool show, bool centerScreen = false)
        {
            if (show)
            {
                this.Size = _windowSize;

                if (centerScreen)
                {
                    //move settings window to the center of the screen
                    this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width / 2) - (this.Width / 2), (Screen.PrimaryScreen.WorkingArea.Height / 2) - (this.Height / 2));
                }
                else
                {
                    //move settings window to bottom right corner of the screen
                    this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                }

                PopulateScreenFromSettings(Statics.ReadSettingsFile());
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }
        private bool CheckSettingsFile()
        {
            //read settings file
            _settings = Statics.ReadSettingsFile();

            if (_settings == null || _settings.Options == null)
            {
                _settings = new BloodSpider.Services.Common.Settings();
                _settings.Options.AddOptionsRow(!chkAutoUpload.Checked, chkShowNotifications.Checked, chkAutoReportErrors.Checked);
                _settings.Login.AddLoginRow(string.Empty, string.Empty);

                return false;
            }

            return true;
        }

        private static void pipeWrite(string command, string Text1, string Text2, int icon)
        {
            //icon = (1 = Info, 2 = Warning, 3 = Error)
            NamedPipeClientStream pipeServerOut = null;
            try
            {
                pipeServerOut = new NamedPipeClientStream(".", "pipeBloodSpiderDetectorIn");
                if (!pipeServerOut.IsConnected)
                    pipeServerOut.Connect(2000);

                StreamString ss = new StreamString(pipeServerOut);

                //verify server identity
                if (ss.ReadString() == "BloodSpider_Service")
                {
                    //talking to the correct service so send the message
                    ss.WriteString(command + "|" + Text1 + "|" + Text2 + "|" + (icon == 0 ? "" : icon.ToString()));
                }
            }//try
            catch (System.TimeoutException tex)
            {
                Error("003", tex);
            }
            catch (Exception ex)
            {
                Error("004", ex);
            }
            finally
            {
                pipeServerOut.Close();
            }
        }
        protected static void ReportBug(string ErrorCode, string StackTrace, string Message)
        {
            string reportString = ErrorCode.Trim() + "|" + StackTrace.Trim() + "|" + Message.Trim() + "|" + Assembly.GetExecutingAssembly().GetName().Version.ToString().Trim();
            pipeWrite("REPORT_BUG", reportString, string.Empty, 0);
        }
    }

    public class BalloonParameters
    {
        public int Timeout { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public ToolTipIcon Icon { get; set; }

        public BalloonParameters(int timeout, string title, string message, ToolTipIcon icon)
        {
            Timeout = timeout;
            Title = title;
            Message = message;
            Icon = icon;
        }
    }
}
