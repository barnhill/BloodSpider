using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters.iSens
{
    public class Caresens_N: AbstractMeter, IMeter
    {
        string _TempString = String.Empty;
        UTF8Encoding encoding = new UTF8Encoding();
        List<byte> bytesRead = new List<byte>();
        static bool _ReadFinished;
        byte[] message = new byte[7];

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public Caresens_N()
        {
            ID = 7;
            MeterDescription = "i-Sens Caresens N";

            //defaults since meter does not communicate these ... or we dont know about how to tell yet
            SampleFormat = Communication.SampleFormat.MGDL;
            SerialNumber = "Not on meter";
            SampleCount = 250;
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
            if (Port != null)
            {
                Dispose();
            }

            Port = new SerialPort(COMport, 9600, Parity.None, 8, StopBits.One);
            Port.ReadBufferSize = 8096;

            return Open();
        }

        public bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            bytesRead.Clear();

            WriteCommand(new byte [] { 0x80 });

            //check for a response
            DateTime start = DateTime.Now;
            while (Port.BytesToRead < 3 && (start - DateTime.Now).TotalMilliseconds < 500)
            {
                Thread.Sleep(10);
            }

            if (Port == null || !Port.IsOpen || Port.BytesToRead < 3)
            {
                return false;
            }
            else
            {
                bytesRead.AddRange(encoding.GetBytes(Port.ReadExisting()));

                if (bytesRead[0] == 0x3f && bytesRead[1] == 0x10 && bytesRead[2] == 0x20)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ReadData()
        {
            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            _TempString = String.Empty;
            bytesRead.Clear();

            _ReadFinished = false;

            if (ReadHeader())
            {
                if (IsMeterConnected(Port.PortName))
                {
                    _ReadFinished = false;

                    //call to loop through memory addresses
                    ReadAllReadings();
                }
            }
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Port.Encoding = ASCIIEncoding.ASCII;
            string newData = Port.ReadExisting();
            _TempString += newData;
    
            bytesRead.AddRange(encoding.GetBytes(newData));

            if ((bytesRead.Count == 0x18 && bytesRead[bytesRead.Count - 1] == 0x20 && bytesRead[bytesRead.Count - 2] == 0x10 && bytesRead[bytesRead.Count - 3] == 0x3f) ||
                (bytesRead.Count == 0x18 && bytesRead[bytesRead.Count - 1] == 0x2f && bytesRead[bytesRead.Count - 2] == 0x1f && bytesRead[bytesRead.Count - 3] == 0x3f))
            {
                _ReadFinished = true;
            }
            else
            {
                _ReadFinished = false;
            }
        }

        public void DataReceived_Header(object sender, SerialDataReceivedEventArgs e)
        {
            Port.Encoding = ASCIIEncoding.ASCII;
            string newData = Port.ReadExisting();
            _TempString += newData;

            bytesRead.AddRange(encoding.GetBytes(newData));

            if (bytesRead.Count == 0xfc && bytesRead[bytesRead.Count - 1] == 0x20 && bytesRead[bytesRead.Count - 2] == 0x10)
            {
                _ReadFinished = true;
            }
            else
            {
                _ReadFinished = false;
            }
        }

        private bool ReadHeader()
        {
            bytesRead.Clear();
            byte[] bytes = new byte[] { 0x8b, 0x11, 0x20, 0x13, 0x24, 0x15, 0x24 };

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived_Header);

            //queue the reading of a row
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkMethod), _autoResetEvent);

            WriteCommand(bytes);

            //block while reading the current row
            if (_autoResetEvent.WaitOne(3000, false))
            {
                Console.WriteLine("Read Header");

                OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ReadAllReadings()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived_Header);
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            byte top = 0x02;
            byte mid = 0x00;
            bool bottomFlag = false;

            byte [] baseMessage = new byte[] { 0x8b, 0x1e, 0x20, 0x10, 0x20, 0x10, 0x28 };

            Port.ReceivedBytesThreshold = 1;

            for (int i = 0; i <= 250; i++)
            {
                baseMessage.CopyTo(message, 0);
                
                //calc top byte
                message[2] = (byte)(message[2] + top);

                //calc mid byte
                message[3] = (byte)(message[3] + mid);

                //calc bot byte
                message[4] = (byte)(message[4] + (byte)(bottomFlag ? 0x08 : 0x00));

                //increment values
                if (bottomFlag)
                {
                    mid++;
                }

                bottomFlag = !bottomFlag;

                if (mid > 0x0f)
                {
                    top++;
                    mid = 0x00;
                }

                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
                bytesRead.Clear();
                _ReadFinished = false;

                //queue the reading of a row
                ThreadPool.QueueUserWorkItem(new WaitCallback(WorkMethod), _autoResetEvent);
                
                WriteCommand(message);

                //block while reading the current row
                if (_autoResetEvent.WaitOne(500, false))
                {
                    //row read successfully
                    Console.WriteLine(string.Format("Read {0}: {1}", i.ToString(), encoding.GetString(bytesRead.ToArray())));

                    //check for ending indicator and dont record it
                    if (GetValue(bytesRead.ToArray(), 1, 2) != 0xff)
                        OnRecordRead(new RecordReadEventArgs(ParseDataRecord(bytesRead.ToArray())));
                }
                else
                {
                    //read timed out
                    break;
                }

                //stop reading if end record hit
                if (GetValue(bytesRead.ToArray(), 1, 2) == 0xff)
                {
                    OnReadFinished(new ReadFinishedEventArgs(this));
                    break;
                }
            }
        }

        private Records.RecordRow ParseDataRecord(byte [] data)
        {
            DateTime dtTimeStamp = new DateTime();
            int iGlucose = 0;
            string strUnits = string.Empty;

            if (data.Length != 0x18) 
                return null;

            int year    = GetValue(data, 1, 2) + 2000;
            int month   = GetValue(data, 4, 5);
            int day     = GetValue(data, 7, 8);
            int hour    = GetValue(data, 10, 11);
            int minutes = GetValue(data, 13, 14);
            int seconds = GetValue(data, 16, 17);

            dtTimeStamp = new DateTime(year, month, day, hour, minutes, seconds);

            iGlucose = GetValue(data, 19, 20);
            
            return Records.AddRecordRow(dtTimeStamp, iGlucose, SampleFormat == SampleFormat.MGDL ? "mg/dl" : "mmol");
        }

        /// <summary>
        /// Extraction of data from each data frame
        /// </summary>
        /// <param name="data">raw data in bytes</param>
        /// <param name="x">byte number for top data byte</param>
        /// <param name="y">byte number for the lower data byte</param>
        /// <returns></returns>
        private byte GetValue(byte [] data, int x, int y)
        {
            byte top = (byte)(GetLowerHalf(data[x]) << (byte)4);
            byte low = GetLowerHalf(data[y]);
            return (byte)(top | low);
        }

        /// <summary>
        /// Bitwise shift operation to get the lower half value of a byte
        /// </summary>
        /// <param name="b">byte to strip the top 4 bits off</param>
        /// <returns>The lower 4 bits of a byte</returns>
        private byte GetLowerHalf(byte b)
        {
            byte left = (byte)(b << (byte)4);
            byte corrected = (byte)(left >> (byte)4);
            return corrected;
        }

        private static void WorkMethod(object stateInfo)
        {
            DateTime start = DateTime.Now;
            while (!_ReadFinished)
            {
                //do nothing but sleep and recheck
                Thread.Sleep(10);
            }

            //signal that read has finished
            ((AutoResetEvent)stateInfo).Set();
        }

        private void WriteCommand(string command)
        {
            //write the command to the serial port
            WriteCommand(encoding.GetBytes(command));
        }

        private void WriteCommand(byte [] bytes)
        {
            //clear the buffers before transmitting
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            //write the command to the serial port
            Port.Write(bytes, 0, bytes.Length);
        }
    }
}
