using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace BloodSpider.Communication.Meters.LifeScan
{
    public class OneTouch_UltraMini: AbstractMeter, IMeter
    {
        string currentCommand = String.Empty;
        int numread = 0;
        string format = string.Empty;
        string softwareVersion = string.Empty;

        public OneTouch_UltraMini()
        {
            ID = 5;
            MeterDescription = "LifeScan One Touch Ultra Mini";
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
            if (!Port.IsOpen)
                Connect(COMport);

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            SendCommand("soft");

            for (int i = 0; i < 25; i++)
            {
                if (Port.BytesToRead > 0)
                {
                    bool found = Port.ReadExisting().Contains("?A");

                    Dispose();

                    return found;
                }

                Thread.Sleep(10);
            }

            Dispose();

            return false;
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(75);

            if (!Port.IsOpen)
                return;

            RawData += Port.ReadExisting();

            byte[] data = Statics.StrToByteArray(RawData);

            if (RawData.Contains(((char)0x03).ToString()) && RawData.Length >= RawData.IndexOf(((char)0x03).ToString()) + 3)
            {
                switch (currentCommand)
                {
                    case "dis": //disconnect command
                        Console.WriteLine("Disconnect");
                        break;
                    case "soft": //read software version command
                        softwareVersion = Statics.TrimNonPrintableCharacters(RawData.Substring(RawData.IndexOf("?A") + 2, RawData.IndexOf("?", RawData.IndexOf("?A") + 2) - (RawData.IndexOf("?A") + 2)));
#if DEBUG
                        Console.WriteLine("Software Version: " + softwareVersion);
#endif
                        break;
                    case "serial": //read serial number from meter
                        SerialNumber = Statics.TrimNonPrintableCharacters(RawData.Substring(RawData.IndexOf("?A") + 2, RawData.IndexOf("?", RawData.IndexOf("?A") + 2) - (RawData.IndexOf("?A") + 2)));
#if DEBUG
                        Console.WriteLine("Serial Number: " + SerialNumber);
#endif
                        break;
                    case "count": //read serial number from meter
                        string [] countReply = RawData.Split(new char[] { (char)AsciiCodes.STX });
                        byte[] dataReply = Statics.StrToByteArray(countReply[countReply.Length - 1]);

                        SampleCount = 0;
                        SampleCount = dataReply[3];
                        SampleCount = SampleCount << 8;
                        SampleCount += dataReply[2];

                        numread = 0;

                        OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));
                        
                        Console.WriteLine("Sample Count: " + SampleCount);
                        break;
                    case "format":
                        Console.WriteLine("Format: " + RawData);
                        byte[] formatReply = Statics.StrToByteArray(RawData);
                        SampleFormat = (formatReply[0x0b] == 0) ? SampleFormat.MGDL : SampleFormat.MMOL;
                        break;
                    case "read":
                        byte[] readReply = Statics.StrToByteArray(RawData);

                        //get date out of bytes
                        long iDate = readReply[0x0e];
                        iDate = iDate << 8;
                        iDate += readReply[0x0d];
                        iDate = iDate << 8;
                        iDate += readReply[0x0c];
                        iDate = iDate << 8;
                        iDate += readReply[0x0b];

                        DateTime timestamp = UnixTimeStampToDateTime(iDate);
                        double hexrep = ConvertToTimestamp(timestamp);

                        int iGlucose = readReply[0x12];
                        iGlucose = iGlucose << 8;
                        iGlucose += readReply[0x11];
                        iGlucose = iGlucose << 8;
                        iGlucose += readReply[0x10];
                        iGlucose = iGlucose << 8;
                        iGlucose += readReply[0x0f];

#if DEBUG
                        Console.WriteLine("Record: " + timestamp + " " + iGlucose + " " + (SampleFormat == SampleFormat.MGDL ? "mg/dl" : "mmol"));
#endif

                        OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(timestamp, iGlucose, (SampleFormat == SampleFormat.MGDL ? "mg/dl" : "mmol"))));

                        if (++numread >= SampleCount)
                        {
                            Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
                            OnReadFinished(new ReadFinishedEventArgs(this));
                            Dispose();
                        }//if

                        break;
                    default:
                        break;
                }//switch

                currentCommand = String.Empty;

                if (Port.IsOpen)
                {
                    Port.DiscardInBuffer();
                }//if

                RawData = String.Empty;
            }//if
        }

        public void ReadData()
        {
            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            currentCommand = String.Empty;

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            SendCommand("soft");
            SendCommand("serial");
            SendCommand("ack");
            SendCommand("count");
            SendCommand("format");

            //reset the counter
            numread = 0;

            Thread.Sleep(1000);

            Records.Clear();

            //send individual reads for each glucose entry
            for (int i = 0; i < SampleCount; i++)
                SendCommand("read", i);
        }
        
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }
        
        private void SendCommand(string cmd)
        {
            SendCommand(cmd, 0);
        }
        private void SendCommand(string cmd, int arg)
        {
            //wait till another command finishes
            while (currentCommand != String.Empty)
                Thread.Sleep(10);

            if (!Port.IsOpen)
                Connect(Port.PortName);

            //clear buffers
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
            RawData = String.Empty;

            byte[] command = new byte[1];
            currentCommand = cmd;

            switch (cmd)
            {
                case "ack": //ack command
                    command = new byte[] { 0x02, 0x06, 0x07, 0x03 };
                    currentCommand = String.Empty;
                    break;
                case "dis": //disconnect command
                    command = new byte[] { 0x02, 0x06, 0x08, 0x03 };
                    break;
                case "soft": //read software version command
                    command = new byte[] { 0x02, 0x09, 0x00, 0x05, 0x0d, 0x02, 0x03 };
                    break;
                case "serial": //read serial number from meter
                    command = new byte[] { 0x02, 0x12, 0x00, 0x05, 0x0B, 0x02, 0x00, 0x00, 0x00, 0x00, 0x84, 0x6A, 0xE8, 0x73, 0x00, 0x03 };
                    break;
                case "count": //read serial number from meter
                    command = new byte[] { 0x02, 0x0A, 0x00, 0x05, 0x1F, 0xF5, 0x01, 0x03 };
                    break;
                case "read": //read glucose value
                    command = new byte[] { 0x02, 0x0A, 0x03, 0x05, 0x1F, 0x00, 0x00, 0x03 };
                    command[5] = (byte)((arg << 8) >> 8);
                    command[6] = (byte)((arg >> 8) >> 8);
                    break;
                case "format": //get record format (mg/dl or mmol)
                    command = new byte[] { 0x02, 0x0E, 0x00, 0x05, 0x09, 0x02, 0x09, 0x00, 0x00, 0x00, 0x00, 0x03 };
                    break;
                default:
                    break;
            }

            //calculate the checksum
            byte[] crc = crc_calculate_crc(command);

            //add the checksum on the end of the command
            Array.Resize<byte>(ref command, command.Length + 2);

            command[command.Length - 2] = crc[0];
            command[command.Length - 1] = crc[1];

            Port.Write(command, 0, command.Length);
        }
        private double ConvertToTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds;
        }
        private bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }
        private byte[] crc_calculate_crc(byte[] buffer)
        {
            ushort length = Convert.ToUInt16(buffer.Length);
            ushort index = 0;
            ushort crc = 0xffff;
            if (buffer != null)
            {
                for (index = 0; index < length; index++)
                {
                    crc = (ushort)((byte)(crc >> 8) | (ushort)(crc << 8));
                    crc ^= buffer[index];
                    ushort temp = ((ushort)(((ushort)crc) & ((ushort)0xff)));
                    int temp2 = temp >> 4;
                    crc ^= (ushort)temp2;
                    crc ^= (ushort)((ushort)(crc << 8) << 4);
                    crc ^= (ushort)((ushort)((crc & 0xff) << 4) << 1);
                }
            }

            byte[] returnval = new byte[2];
            returnval[1] = (byte)(crc >> 8);
            returnval[0] = (byte)(crc & 0xff);

            return (returnval);
        }
    }
}
