using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace BloodSpider.Communication.Meters.Abbott
{
    public class FreeStyle: AbstractMeter, IMeter
    {
        static bool _HeaderRead;
        static bool _TestMode;

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

        public bool Open()
        {
            Port.DtrEnable = true;

            try
            {
                if (!Port.IsOpen)
                {
                    Thread.Sleep(250);
                    Port.Open();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Port.IsOpen;
            }

            Thread.Sleep(250);

            //clear the buffers
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
            Port.BaseStream.Flush();

            return Port.IsOpen;
        }

        public bool Connect(string COMport)
        {
            base.Close();

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            Port = new SerialPort(COMport, 19200, Parity.None, 8, StopBits.One);
            Port.ReadBufferSize = 8096;
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            return Open();
        }

        public bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            ReadData(true);

            _HeaderRead = false;

            return !string.IsNullOrEmpty(SerialNumber);
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Port == null || !Port.IsOpen)
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
                        if (line.Contains("END"))
                        {
                            RawData = String.Empty;
                            _autoResetEvent.Set();
                            
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
                
#if DEBUG
                Console.WriteLine("SERIAL: " + SerialNumber);
#endif

                //set sample count
                SampleCount = int.Parse(header[4]);

                OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));
                
            }//if
        }

        public void ReadData(bool testMode)
        {
            _TestMode = testMode;
            
            _HeaderRead = false;

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            
            //clear buffers
            Connect(Port.PortName);

            Port.DiscardOutBuffer();
            while (Port.BytesToRead != 0)
            {
                Port.DiscardInBuffer();
            }

            RawData = String.Empty;

            //send the memory dump command to the device
            Port.Write("mem");

            Thread.Sleep(100);

            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadHelper), _autoResetEvent);

            // Wait for work method to signal.
            if (_autoResetEvent.WaitOne(60000, false))
            {
                //Console.WriteLine("Read Finished Successfully");
                Thread.Sleep(1000);
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                
                if (!_TestMode)
                    OnReadFinished(new ReadFinishedEventArgs(this));
                
                Dispose();
            }
            else
            {
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                Dispose();
                Console.WriteLine("Timed out waiting for meter to finish reading.");
            }
        }

        public void ReadData()
        {
            ReadData(false);
        }

        private static void ReadHelper(object stateInfo)
        {
            while (true)
            {
                Thread.Sleep(100);
            }
        }

    }
}
