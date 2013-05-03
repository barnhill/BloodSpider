using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters.Abbott
{
    public class FreeStyle: AbstractMeter, IMeter
    {
        static bool _HeaderRead;
        static bool _TestMode;
        static bool _TestPassed;
        static bool _ReadFinished;

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        private void OnEvent(object sender, EventArgs e)
        {
            _autoResetEvent.Set();
        }

        public FreeStyle()
        {
            SampleFormat = SampleFormat.MGDL;
            ID = 3;
            MeterDescription = "Abbott Freestyle";
        }

        public override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Port == null)
                return;

            RawData += Port.ReadExisting();

            //header already read so grab the records
            if (_HeaderRead)
            {
                using (StringReader reader = new StringReader(RawData))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //end record encountered
                        if (RawData.Contains("END"))
                        {
                            _ReadFinished = true;
                            
                            break;
                        }//if
                        else
                        {
                            //not end of data file so parse it
                            string[] linesplit = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            //add line if its not the end line
                            if (line != Environment.NewLine && !line.Contains("END") && linesplit.Length > 0 && linesplit[linesplit.Length - 1] == "0x00")
                            {
                                RawData = RawData.Replace(line + "\r\n", "");

                                DateTime dtTimeStamp = DateTime.Parse(line.Substring(5, 18), CultureInfo.InvariantCulture);

                                try
                                {
                                    if (Records.FindByTimestamp(dtTimeStamp) == null)
                                    {
#if DEBUG                                        
                                        Console.WriteLine("Record: " + line); 
#endif
                                        OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(dtTimeStamp, Int32.Parse(linesplit[0]), SampleFormat == SampleFormat.MGDL ? "mg/dl" :  "mmol")));
                                    }//if
                                    else
                                    {
#if DEBUG
                                        Console.WriteLine("DUPLIC: " + line);
#endif
                                    }//else
                                }//try
                                catch 
                                {
#if DEBUG
                                    //glucose was not a number (ex. LO)
                                    Console.WriteLine("NOTNUM: " + line);
#endif
                                }//catch

                                line = null;
                            }//if
                        }//else
                    }//while
                }//using
            }//if

            //header record is present
            if (!_HeaderRead && RawData.Contains("\r\n\n"))
            {
                string[] headeranddata = RawData.Split(new string[] { "\r\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] header = headeranddata[0].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

#if DEBUG
                Console.WriteLine("Header: " + headeranddata[0]);
#endif  

                RawData = (headeranddata.Length > 1) ? headeranddata[1] : String.Empty;

                //set header read
                _HeaderRead = true;

                //set serial number
                SerialNumber = header[1];

                if (_TestMode)
                {
                    _TestPassed = true;
                    return;
                }

#if DEBUG
                Console.WriteLine("SERIAL: " + SerialNumber);
#endif

                //set sample count
                SampleCount = int.Parse(header[4]);

                DeviceInfo d = new DeviceInfo();
                
                OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));
            }//if
        }

        public void ReadData(bool testMode)
        {
            _TestMode = testMode;

            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadHelper), _autoResetEvent);

            _ReadFinished = false;
            _TestPassed = false;
            _HeaderRead = false;

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            
            //clear buffers
            Connect(Port.PortName);

            Port.DiscardOutBuffer();
            while (Port.BytesToRead != 0)
            {
                Port.DiscardInBuffer();
            }

            //send the memory dump command to the device
            Port.Write("mem");

            Thread.Sleep(1000);

            // Wait for work method to signal.
            if (_autoResetEvent.WaitOne(_TestMode ? 5000 : 30000, false))
            {
                Console.WriteLine("Read Finished Successfully");
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);

                OnReadFinished(new ReadFinishedEventArgs(this));
                Dispose();
            }
            else
            {
                Console.WriteLine("Timed out waiting for meter to finish reading.");
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            }
        }

        public override void ReadData()
        {
            ReadData(false);
        }

        private static void ReadHelper(object stateInfo)
        {
            DateTime start = DateTime.Now;
            while (!_ReadFinished && !_TestPassed && (DateTime.Now-start).TotalMilliseconds < 30000)
            {
                //do nothing but sleep and recheck
                Thread.Sleep(100);
            }

            //signal that read has finished
            ((AutoResetEvent)stateInfo).Set();
        }

        public override bool Connect(string COMport)
        {
            base.Close();

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            Port = new SerialPort(COMport, 19200, Parity.None, 8, StopBits.One);
            Port.ReadBufferSize = 8096;
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            return base.Open();
        }

        public override bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            ReadData(true);

            _TestMode = false;
            _HeaderRead = false;
            _TestPassed = false;

            return !string.IsNullOrEmpty(SerialNumber);
        }
    }
}
