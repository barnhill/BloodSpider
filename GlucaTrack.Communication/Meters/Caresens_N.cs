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
        byte[] bytesRead;
        bool _TestMode;

        public Caresens_N()
        {
            ID = 7;
            MeterDescription = "i-Sens Caresens N";
        }

        public override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            bytesRead = encoding.GetBytes(Port.ReadExisting());
            Console.Write(encoding.GetString(bytesRead));
        }

        public override void ReadData()
        {
            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            _TempString = String.Empty;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            _TestMode = false;

            WriteCommand(new byte[] { 0x8b, 0x1e, 0x22, 0x10, 0x20, 0x10, 0x28 });
        }
        
        public override bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            _TestMode = true;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

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
                UTF8Encoding encoding = new UTF8Encoding();
                bytesRead = encoding.GetBytes(Port.ReadExisting());

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

        private void WriteCommand(string command)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            //write the command to the serial port
            WriteCommand(encoding.GetBytes(command));
        }

        private void WriteCommand(byte [] bytes)
        {
            //write the command to the serial port
            Port.Write(bytes, 0, bytes.Length);
        }
    }
}
