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
        static bool _ReadRecordFinished;
        byte[] message = new byte[7];

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public Caresens_N()
        {
            ID = 7;
            MeterDescription = "i-Sens Caresens N";
        }

        public override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Port.Encoding = ASCIIEncoding.ASCII;
            string newData = Port.ReadExisting();
            _TempString += newData;
    
            bytesRead.AddRange(encoding.GetBytes(newData));

            if (bytesRead.Count == 0x18 && bytesRead[bytesRead.Count - 1] == 0x20 && bytesRead[bytesRead.Count - 2] == 0x10 && bytesRead[bytesRead.Count - 3] == 0x3f)
            {
                _ReadFinished = true;
            }
            else
            {
                _ReadFinished = false;
            }
            //_ReadFinished = true;
            //foreach (byte b in bytesRead)
            //{
            //    if ((b & 0x0f) != 0x0f)
            //    {
            //        _ReadFinished = false;
            //        break;
            //    }
            //}

            //if (_ReadFinished)
            //{
            //    OnReadFinished(new ReadFinishedEventArgs(this));
            //}
            //TODO: break each rows data up and send it to OnRecordRead

            //OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(dtTimeStamp, Int32.Parse(linesplit[0]), SampleFormat == SampleFormat.MGDL ? "mg/dl" : "mmol")));
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

            //TODO: break header data up and populate the header record

            //OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(dtTimeStamp, Int32.Parse(linesplit[0]), SampleFormat == SampleFormat.MGDL ? "mg/dl" : "mmol")));
        }

        public override void ReadData()
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

        public override bool IsMeterConnected(string COMport)
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
                OnHeaderRead(new HeaderReadEventArgs(9999, this));
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

            for (int i = 0; i < 250; i++)
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
                }
                else
                {
                    //read timed out
                    break;
                }

                //TODO: break reading if end record hit
            }
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
