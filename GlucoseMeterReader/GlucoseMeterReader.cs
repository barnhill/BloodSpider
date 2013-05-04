using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using GlucaTrack.Communication;
using GlucaTrack.Communication.Meters;

using USBHIDDRIVER.USB;

namespace GlucoseMeterReader
{
    public partial class GlucoseMeterReader : Form
    {
        private delegate void delegateAppendTextToResponseDisplay(string text);
        private delegate void delegateParseDataToDataset(string text);
        private delegate void delegateSetDataSource(Records.RecordDataTable source);
        private delegate void delegateSetProgressBarMax(int max);
        private delegate void delegateSetProgressBarValue(int value);
        private delegate void delegateSetReadTimeValue(double value);
        private delegate void delegateSetHeaderInfo(string serial, string numSamples);

        DateTime startDateTime = new DateTime();
        WaitMessage wm = new WaitMessage();
        BackgroundWorker bgw = new BackgroundWorker();

        int RecordsRead = 0;

        IMeter Meter;

        public GlucoseMeterReader()
        {
            InitializeComponent();

#if !DEBUG
            scLeftRight.Panel2Collapsed = true;
            this.Width = 300;
            this.Height = 300;
#endif

            PopulateComports();
            PopulateMeterTypes();
        }

        private void PopulateComports()
        {
            foreach (string comport in SerialPort.GetPortNames())
            {
                this.cbComports.Items.Add(comport);
            }//foreach
        }

        private void PopulateMeterTypes()
        {
            Statics.PopulateMeterTypes();
            this.cbMeterType.DataSource = new BindingSource(Statics.MeterToClassLookup, null);
            this.cbMeterType.DisplayMember = "key";
            this.cbMeterType.ValueMember = "value";
        }
        
        private void btnRead_Click(object sender, EventArgs e)
        {
            RecordsRead = 0;

            //clear the form
            dgvData.DataSource = null;
            dgvData.Visible = false;
            tableLayoutPanel1.Visible = false;
            dgvData.Hide();
            this.textConsole.Text = String.Empty;
            this.lblReadTime.Text = String.Empty;
            this.lblSerialNumber.Text = String.Empty;
            this.lblNumSamplesOnMeter.Text = String.Empty;

            //show wait message
            Point startLocationOfWaitWindow = this.Location;
            startLocationOfWaitWindow.X += (this.Width / 2) - (wm.Width / 2);
            startLocationOfWaitWindow.Y += (this.Height / 2) - (wm.Height / 2);
            wm.Location = startLocationOfWaitWindow;
            wm.ProgressValue = 0;
            
            if (!wm.Visible)
                wm.Show(this);

            wm.Update();

            //record the start time
            startDateTime = DateTime.Now;

            bgw.Dispose();
            bgw = new BackgroundWorker();
            bgw.WorkerSupportsCancellation = true;
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerAsync(new string [] { this.cbComports.SelectedItem.ToString(), cbMeterType.SelectedValue.ToString().Trim() });
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Thread Completed");
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Meter != null && (!Meter.Port.IsOpen || Meter.Port.PortName != ((string[])e.Argument)[0]))
            {
                Meter.Dispose();
                Meter = null;
                System.Threading.Thread.Sleep(2500);
            }

            Meter = (IMeter)Activator.CreateInstance(Type.GetType("GlucaTrack.Communication.Meters." + ((string[])e.Argument)[1] + ", GlucaTrack.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            Meter.ReadFinished += new EventHandler(OnReadFinished);
            Meter.RecordRead += new EventHandler(OnRecordRead);
            Meter.HeaderRead += new EventHandler(OnHeaderRead);

            Console.WriteLine("Connecting to " + ((string[])e.Argument)[0]);

            if (!Meter.Port.IsOpen)
            {
                Meter.Connect(((string[])e.Argument)[0]);
            }

            if (Meter.IsMeterConnected(((string[])e.Argument)[0]))
            {
                Meter.Port.DiscardInBuffer();
                Meter.Port.DiscardOutBuffer();
                Meter.ReadData();
            }
        }

        void dgvData_SetDataSource(Records.RecordDataTable source)
        {
            dgvData.DataSource = Meter.Records;
            dgvData.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvData.Show();
        }

        protected virtual void OnRecordRead(object sender, EventArgs e)
        {
            delegateSetProgressBarValue dspbm = new delegateSetProgressBarValue(SetProgressBarValue);
            delegateAppendTextToResponseDisplay attrd = new delegateAppendTextToResponseDisplay(AppendTextToDisplay);

            if (InvokeRequired)
            {
                Invoke(dspbm, new object[] { RecordsRead < Meter.SampleCount ? ++RecordsRead : Meter.SampleCount });
                Invoke(attrd, new object[] { RecordsRead.ToString() + ". " + ((RecordReadEventArgs)e).Row.Timestamp.ToString() + " | " + ((RecordReadEventArgs)e).Row.Glucose.ToString() + " | " + ((RecordReadEventArgs)e).Row.Units.ToString() + Environment.NewLine });
            }//if
            else
            {
                dspbm(RecordsRead < Meter.SampleCount ? ++RecordsRead : Meter.SampleCount);
                attrd(((RecordReadEventArgs)e).Row.Timestamp.ToString() + " | " + ((RecordReadEventArgs)e).Row.Glucose.ToString() + " | " + ((RecordReadEventArgs)e).Row.Units.ToString() + Environment.NewLine);
            }//else
        }

        protected virtual void OnHeaderRead(object sender, EventArgs e)
        {
            delegateSetProgressBarMax dspbm = new delegateSetProgressBarMax(SetProgressBarMax);
            delegateSetHeaderInfo setHeaderInfo = new delegateSetHeaderInfo(SetHeaderInfo);

            if (InvokeRequired)
            {
                Invoke(dspbm, new object[] { ((HeaderReadEventArgs)e).RowCount });
                Invoke(setHeaderInfo, new object[] { Meter.SerialNumber, Meter.SampleCount.ToString() });
            }//if
            else
            {
                dspbm(((HeaderReadEventArgs)e).RowCount);
                setHeaderInfo(Meter.SerialNumber, Meter.SampleCount.ToString());
            }//else
        }

        protected virtual void OnReadFinished(object sender, EventArgs e)
        {
            delegateSetDataSource stds = new delegateSetDataSource(dgvData_SetDataSource);
            delegateSetReadTimeValue srtv = new delegateSetReadTimeValue(SetReadTimeValue);

            if (InvokeRequired)
            {
                Invoke(stds, new object[] { Meter.Records });
                Invoke(srtv, new object[] { Meter.ReadTime });
            }//if
            else
            {
                stds(Meter.Records);
                srtv(Meter.ReadTime);
            }//else

            Meter.Close();
        }

        private void GlucoseMeterReader_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Meter != null)
                    Meter.Port.Close();
            }//try
            catch { }
        }

        private void SetProgressBarMax(int max)
        {
            this.wm.ProgressMax = max;
        }

        private void SetProgressBarValue(int value)
        {
            this.wm.ProgressValue = value;
            this.wm.Invalidate();
            this.wm.Update();
        }

        private void AppendTextToDisplay(string text)
        {
            this.textConsole.Text = text + this.textConsole.Text;
        }

        private void SetReadTimeValue(double value)
        {
            this.wm.ProgressValue = dgvData.Rows.Count - 1;
            this.lblReadTime.Text = value.ToString() + "s";

            this.lblPerSecond.Text = Math.Round((double)(dgvData.Rows.Count - 1) / value, 1).ToString() + " per sec";

            this.wm.StartCloseTimer();
        }

        private void SetHeaderInfo(string serial, string numSamples)
        {
            lblSerialNumber.Text = serial;
            lblStatus.Text = "SN: " + serial;
            lblNumSamplesOnMeter.Text = numSamples;

            tableLayoutPanel1.Visible = true;
        }

        USBHIDDRIVER.USBInterface usb;
        int commandindex = 1;
        List<byte[]> commands = new List<byte[]>();
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

        private void btnRocheGetData_Click(object sender, EventArgs e)
        {
            usb = new USBHIDDRIVER.USBInterface("vid_173a", "pid_2106");
            
            string [] devices = usb.getDeviceList();
            usb.enableUsbBufferEvent(new EventHandler(Accuchek_ReceivedData));
            System.Threading.Thread.Sleep(100);
            Console.WriteLine("Connect: " + usb.Connect().ToString());

            //string temp = Convert.ToString(Convert.ToChar(01));
            //temp += Convert.ToString(Convert.ToChar(44));
            //temp += Convert.ToString(Convert.ToChar(01));

            //commandindex = 1;
            //commands.Clear();

            //commands.Add(temp);
            //commands.Add("\n");
            //commands.Add("C4\n");
            //commands.Add("C 3\n");
            //commands.Add("S 1\n");
            //commands.Add("S 2\n");
            //commands.Add("`\n");
            //commands.Add("S 3\n");
            
            //Send_AccuChek_Command(commandindex++);

            commands.Add(new byte[] { 0x01, 0x13 });
            commands.Add(new byte[] { 0x01 ,0x44, 0x01});
            commands.Add(new byte[] { 0x01, 0x44, 0x01 });
            commands.Add(new byte[] { 0x01, 0x43, 0x01, 0x02 });
            commands.Add(new byte[] { 0x01, 0x45 });
            commands.Add(new byte[] { 0x01, 0x43, 0x04, 0x01 });
            commands.Add(new byte[] { 0x01, 0x44 });
            Send_AccuChek_Command(commands[commandindex++]);

            usb.startRead();
        }

        private void Accuchek_ReceivedData(object sender, EventArgs e)
        {
            Console.WriteLine("Received");

            USBHIDDRIVER.List.ListWithEvent list = (USBHIDDRIVER.List.ListWithEvent)sender;
            
            foreach (Byte [] b in list)
            {
                for (int i = 0; i < b.Length; i++)
                    Console.Write(b[i].ToString("x").ToUpper());
            }//foreach

            if (commandindex < commands.Count)
            {
                ((USBHIDDRIVER.List.ListWithEvent)sender).Clear();
                Send_AccuChek_Command(commands[commandindex++]);
            }//if
        }

        private void Send_AccuChek_Command(byte [] command)
        {
            //Console.WriteLine("\n\nSending: " + commands[i]);
            //byte[] temp = encoding.GetBytes(commands[i]);
            usb.write(command);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            txtDevicesDetected.Clear();
            txtDevicesDetected.Update();

            DeviceInfo fdi = Statics.DetectFirstDevice();

            if (fdi != null)
                    this.txtDevicesDetected.Text = "Port: " + fdi.ComPortName + Environment.NewLine + "Device: " + fdi.DeviceType.Name.ToString() + Environment.NewLine;
            else
                this.txtDevicesDetected.Text = "No Devices Detected";
        }
    }
}
