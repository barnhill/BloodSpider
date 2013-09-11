using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters.LifeScan
{
    public class OneTouch_Ultra: AbstractMeter, IMeter
    {
        byte[] MeterAckCommand = new byte[] { 0x11, 0x0D, 0x44, 0x4d, 0x53, 0x0d, 0x0d };
        string _TempString = String.Empty;
        bool _MeterResponded = false;
        bool _HeaderRead = false;
        int _RecordCount = 0;

        public OneTouch_Ultra()
        {
            ID = 10;
            MeterDescription = "LifeScan One Touch Ultra";
        }

        public bool Open()
        {
            Port.DtrEnable = true;

            try
            {
                if (!Port.IsOpen)
                {
                    Port.Open();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Port.IsOpen;
            }

            //clear the buffers
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
            Port.BaseStream.Flush();

            return Port.IsOpen;
        }

        public bool Connect(string COMport)
        {
            if (Port != null)
            {
                Dispose();
            }

            Port = new SerialPort(COMport, 9600, Parity.None, 8, StopBits.One);

            return Open();
        }

        public bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            _HeaderRead = false;
            DateTime dtStartTime = DateTime.Now;

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            while (!_MeterResponded && (DateTime.Now - dtStartTime).TotalMilliseconds < 2000)
            {
                WriteCommand("DMS");
                Thread.Sleep(25);
            }//while

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (!_MeterResponded)
            {
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                base.Close();
                Dispose();
                
                return false;
            }

            _MeterResponded = false;
            
            WriteCommand("DM?");

            dtStartTime = DateTime.Now;
            while (!_MeterResponded && (DateTime.Now - dtStartTime).TotalMilliseconds < 1000)
            {
                Thread.Sleep(25);
            }

            if (!_MeterResponded)
            {
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                base.Close();
                Dispose();

                return false;
            }

            _MeterResponded = false;

            WriteCommand("DM@");

            dtStartTime = DateTime.Now;
            while (!_MeterResponded && (DateTime.Now - dtStartTime).TotalMilliseconds < 1000)
            {
                Thread.Sleep(25);
            }

            if (!_MeterResponded)
            {
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                base.Close();
                Dispose();

                return false;
            }

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            return true;
        }
        
        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           if (!Port.IsOpen)
                return;

            _TempString += Port.ReadExisting();

            if (_TempString.Contains("\r\n"))
            {
#if DEBUG
                Console.WriteLine(_TempString);
#endif

                string commandResponse = _TempString.Substring(0, _TempString.IndexOf("\r\n") + 2);
                _TempString = _TempString.Replace(commandResponse, "");

                //========== ismeterconnected request (returns software version) ==========
                if (commandResponse.StartsWith("?"))
                {
                    if (commandResponse.Split(new char [] {'.'}).Length == 3)
                        _MeterResponded = true;
                }

                if (commandResponse.StartsWith("S "))
                {
                    _MeterResponded = true;
                }

                //========== serial number response ==========
                if (commandResponse.StartsWith("@"))
                {
                    _MeterResponded = true;
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

                        OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));
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

                            OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(dtTimeStamp, Int32.Parse(parsedMsg[3].ToString()), "mg/dl")));//always returns data in mg/dl
                        }//try
                        catch { }
                        finally 
                        {
                            ++_RecordCount;
                        }

                        if (_RecordCount == SampleCount)
                        {
                            //all records read so close the port and dispose
                            OnReadFinished(new ReadFinishedEventArgs(this));
                            Dispose();
                        }//if
                    }
                }//elseif
            }//if
        }

        public void ReadData()
        {
            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            _HeaderRead = false;
            _MeterResponded = false;
            DateTime dtStartTime = DateTime.Now;

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            while (!_MeterResponded && (DateTime.Now - dtStartTime).TotalMilliseconds < 2000)
            {
                WriteCommand("DMS");
                Thread.Sleep(25);
            }//while

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (!_MeterResponded)
            {
                Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                base.Close();
                Dispose();

                return;
            }
            else
            {
                WriteCommand("DMP");
                Thread.Sleep(25);
            }
        }

        private void WriteCommand(string command)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            
            string fullcommand = string.Empty;

            //if (!command.EndsWith("DMP"))
            //fullcommand += Convert.ToChar(Int32.Parse("0d", NumberStyles.HexNumber)).ToString();

            fullcommand += Convert.ToChar(Int32.Parse("11", NumberStyles.HexNumber)).ToString();
            fullcommand += Convert.ToChar(Int32.Parse("0d", NumberStyles.HexNumber)).ToString();
            fullcommand += command;

            if (command.EndsWith("DMS") || command.EndsWith("DMT"))
                fullcommand += Convert.ToChar(Int32.Parse("0d", NumberStyles.HexNumber)).ToString();

            if (command.EndsWith("DMS"))
                fullcommand += Convert.ToChar(Int32.Parse("0d", NumberStyles.HexNumber)).ToString();

            //write the command to the serial port
            byte[] arySendBytes = encoding.GetBytes(fullcommand);
            Port.Write(arySendBytes, 0, arySendBytes.Length);
        }
    }
}
