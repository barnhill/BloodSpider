using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters.LifeScan
{
    public class OneTouch_Ultra2: AbstractMeter, IMeter
    {
        byte[] MeterAckCommand = new byte[] { 0x11, 0x0D, 0x44, 0x4d, 0x53, 0x0d, 0x0d };
        string _TempString = String.Empty;
        bool _MeterResponded = false;
        bool _HeaderRead = false;
        bool _TestMode = false;
        int _RecordCount = 0;

        public OneTouch_Ultra2()
        {
            ID = 4;
            MeterDescription = "LifeScan One Touch Ultra 2";
        }

        public override void DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            _MeterResponded = true;

            if (!Port.IsOpen)
                return;

            _TempString += Port.ReadExisting();

            if (_TempString.Contains("\r\n"))
            {
                string commandResponse = _TempString.Substring(0, _TempString.IndexOf("\r\n") + 2);
                _TempString = _TempString.Replace(commandResponse, "");

                //========== serial number response ==========
                if (commandResponse.StartsWith("@"))
                {
                    SerialNumber = commandResponse.Split(new char[] { ' ' })[1].Replace('\"', ' ').Trim();
#if DEBUG
                    Console.WriteLine("SerialNumber: " + SerialNumber);
#endif
                }//if

                //========== patient record response ==========
                else if (commandResponse.StartsWith("P"))
                {
                    if (!_HeaderRead)
                    {
                        //first row of P records is the header
                        _HeaderRead = true;
                        _RecordCount = 0;
                        string[] header = commandResponse.Split(new char[] { ',' });
                        SampleCount = int.Parse(header[0].Split(new char[] { ' ' })[1]);
                        SerialNumber = header[1].Replace("\"", "");
                        SampleFormat = header[2].Substring(header[2].IndexOf('\"') + 1, header[2].LastIndexOf('\"') - (header[2].IndexOf('\"') + 1)).ToLower().Trim() == "mg/dl" ? SampleFormat.MGDL : SampleFormat.MMOL;

#if DEBUG
                        Console.WriteLine("SampleCount: " + SampleCount);
                        Console.WriteLine("SampleFormat: " + SampleFormat.ToString());
                        Console.WriteLine("SerialNumber: " + SerialNumber);
#endif

                        if (_TestMode)
                        {
                            base.Close();
                            Dispose();
                        }

                        OnHeaderRead(new HeaderReadEventArgs(SampleCount));
                    }//if
                    else
                    {
                        //all other P records are glucose records
                        Console.WriteLine("Record: " + commandResponse);
                        string[] parsedMsg = commandResponse.Replace("\"", "").Replace(" ", "").Split(new char[] { ',' });
                        string[] parsedDate = parsedMsg[1].Split(new char[] { '/' });
                        string[] parsedTime = parsedMsg[2].Split(new char[] { ':' });

                        try
                        {
                            DateTime dtTimeStamp = new DateTime(int.Parse(parsedDate[2].ToString()), int.Parse(parsedDate[0].ToString()), int.Parse(parsedDate[1].ToString()), int.Parse(parsedTime[0].ToString()), int.Parse(parsedTime[1].ToString()), int.Parse(parsedTime[2].ToString()));

                            if (dtTimeStamp.Year < 100)
                            {
                                //two digit year encountered (all dates are assumed to be in 2000+)
                                dtTimeStamp = dtTimeStamp.AddYears(2000);
                            }//if

                            OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(dtTimeStamp, Int32.Parse(parsedMsg[3].ToString()), "mg/dl")));
                        }//try
                        catch { }

                        if (++_RecordCount == SampleCount)
                        {
                            //all records read so close the port and dispose
                            OnReadFinished(new ReadFinishedEventArgs(Records));
                            base.Close();
                            Dispose();
                        }//if
                    }
                }//elseif
            }//if
        }

        public override void ReadData()
        {
            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);

            while (!_MeterResponded)
            {
                WriteCommand("DMS");
                Thread.Sleep(25);
            }//while

            _HeaderRead = false;

            Thread.Sleep(1000);

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            WriteCommand("DMP");
        }

        public override bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);

            _HeaderRead = false;
            _TestMode = true;
            DateTime dtStartTime = DateTime.Now;

            while (!_MeterResponded && (DateTime.Now - dtStartTime).TotalMilliseconds < 2000)
            {
                WriteCommand("DMS");
                Thread.Sleep(25);
            }//while

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (!_MeterResponded)
            {
                base.Close();
                Dispose();

                return _MeterResponded;
            }

            Thread.Sleep(1000);

            WriteCommand("DMP");

            dtStartTime = DateTime.Now;
            while (!_HeaderRead && (DateTime.Now - dtStartTime).TotalMilliseconds < 3000)
            {
                Thread.Sleep(100);
            }

            return _HeaderRead;
        }

        private void WriteCommand(string command)
        {
            string fullcommand = Convert.ToChar(Int32.Parse("11", System.Globalization.NumberStyles.HexNumber)).ToString();
            fullcommand += Convert.ToChar(Int32.Parse("0d", System.Globalization.NumberStyles.HexNumber)).ToString();
            fullcommand += command;
            fullcommand += Convert.ToChar(Int32.Parse("0d", System.Globalization.NumberStyles.HexNumber)).ToString();
            fullcommand += Convert.ToChar(Int32.Parse("0d", System.Globalization.NumberStyles.HexNumber)).ToString();

            System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
            
            //write the command to the serial port
            Port.Write(encoding.GetBytes(fullcommand), 0, encoding.GetBytes(fullcommand).Length);
        }
    }
}
