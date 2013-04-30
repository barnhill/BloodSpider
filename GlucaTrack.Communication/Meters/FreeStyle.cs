using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters.Abbott
{
    public class FreeStyle: AbstractMeter, IMeter
    {
        bool _HeaderRead = false;
        bool _TestMode = false;
        bool _TestPassed = false;
        bool _ReadFinished = false;

        public FreeStyle()
        {
            SampleFormat = SampleFormat.MGDL;
            ID = 3;
            MeterDescription = "Abbott Freestyle";
        }

        public override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
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
                        if (line.Contains("END") )
                        {
                            _ReadFinished = true;
                            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);

                            OnReadFinished(new ReadFinishedEventArgs(Records));
                            Close();
                            Dispose();
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

                                DateTime dtTimeStamp = DateTime.Parse(line.Substring(5, 18), System.Globalization.CultureInfo.InvariantCulture);

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
                    Port.DataReceived += null;
                    
                    return;
                }

#if DEBUG
                Console.WriteLine("SERIAL: " + SerialNumber);
#endif

                //set sample count
                SampleCount = int.Parse(header[4]);

                OnHeaderRead(new HeaderReadEventArgs(SampleCount));
            }//if
        }

        public override void ReadData()
        {
            _ReadFinished = false;
            _TestPassed = false;

            if (!Port.IsOpen)
            {
                DateTime startAutoConnect = DateTime.Now;

                string strComport = Port.PortName;
                Port.Dispose();
                Port = null;

                while (!Connect(strComport))
                    Thread.Sleep(50);

                Console.WriteLine("Autoconnect: " + (DateTime.Now - startAutoConnect).TotalSeconds.ToString() + "s");
            }

            //Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            //get the memory dump from the device
            Port.Write("mem");
        }

        public override bool Connect(string COMport)
        {
            base.Close();

            Port = new SerialPort(COMport, 19200, Parity.None, 8, StopBits.One);

            return base.Open();
        }

        public override bool IsMeterConnected(string COMport)
        {
            _TestMode = true;

            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            ReadData();

            DateTime dtStartTime = DateTime.Now;
            while (!_TestPassed && (DateTime.Now - dtStartTime).TotalMilliseconds < 5000)
            {
                Thread.Sleep(100);
            }

            _TestMode = false;
            _HeaderRead = false;
            _TestPassed = false;
            return !string.IsNullOrEmpty(SerialNumber);
        }
    }
}
