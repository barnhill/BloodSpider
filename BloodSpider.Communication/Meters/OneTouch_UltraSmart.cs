using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace BloodSpider.Communication.Meters.LifeScan
{
    public class OneTouch_UltraSmart: AbstractMeter, IMeter
    {
        DateTime AckSentTime = new DateTime();
        string currentCommand = String.Empty;
        bool _EndRecordEncountered = false;
        bool _MeterFound = false;
        int recordIndex = 0;

        public OneTouch_UltraSmart()
        {
            ID = 6;
            MeterDescription = "LifeScan One Touch Ultra Smart";
        }

        public bool Connect(string COMport)
        {
            base.Close();

            Port = new SerialPort(COMport, 38400, Parity.None, 8, StopBits.One);
            Port.Handshake = Handshake.RequestToSend;
            Port.ReadBufferSize = 16384;
            Port.WriteBufferSize = 1024;
            Port.ReadTimeout = 800;
            Port.WriteTimeout = 500;

            return Open();
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

        public bool IsMeterConnected(string COMport)
        {
            _MeterFound = false;

            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            DateTime dtStartTime = DateTime.Now;
            while (!_MeterFound && (DateTime.Now - dtStartTime).TotalMilliseconds < 3000)
            {
                try
                {
                    SendCommand("ack");
                }//try
                catch { }

                System.Threading.Thread.Sleep(100);
            }//while

            SendCommand("serial");

            Thread.Sleep(100);
            
            base.Close();

            return _MeterFound && !string.IsNullOrEmpty(SerialNumber);
        }

        public void DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(20);

            int readNumber = Port.BytesToRead;
            byte[] data = new byte[readNumber];
            Port.Read(data, 0, readNumber);

            RawData += System.Text.Encoding.Default.GetString(data);

            if (RawData.Contains("\r\n") || currentCommand == "read")
            {
                switch (currentCommand)
                {
                    case "test":
                    case "ack": //read software version command
                        _MeterFound = true;
                        break;
                    case "serial": //read serial number from meter
                        SerialNumber = RawData.Split(new char[] { '\"' })[1].Trim();
#if DEBUG
                        Console.WriteLine("SerialNumber: " + SerialNumber);
#endif
                        break;
                    case "soft": //read software version and date
                        string software = RawData.Split(new char[] { ' ' })[0].Substring(2).Trim();
                        OnHeaderRead(new HeaderReadEventArgs(5000, this));
#if DEBUG
                        Console.WriteLine("Software Version: " + software);
#endif
                        break;
                    case "format": //read software version and date
                        string format = RawData.Split(new char[] { '\"' })[1].Trim().ToLower();
                        SampleFormat = (format == "mg/dl") ? SampleFormat.MGDL : SampleFormat.MMOL;
#if DEBUG
                        Console.WriteLine("SampleFormat: " + format);
#endif
                        break;
                    case "count":
                    case "read": //reads the glucose samples

                        //check if End Record was read
                        if (data.Length == 7)
                        {
                            SampleCount = Records.Count;
                            _EndRecordEncountered = true;
                            Port.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
                            OnReadFinished(new ReadFinishedEventArgs(this));
                            Close();
                            Dispose();
                            break;
                        }//if

                        //data = unEscape10s(data);

                        //print raw hex data
                        Console.Write("Record: ");

                        foreach (byte b in data)
                        {
                            Console.Write(b.ToString("X") + " ");
                        }//if
                        Console.Write(Environment.NewLine);

                        //check type byte to make sure its a glucose record
                        if (data[12] == 0)
                        {
                            byte[] packetIDBytes = { data[2], data[3] };
                            UInt16 packetID = BitConverter.ToUInt16(packetIDBytes, 0);

                            //parse glucose record
                            byte[] glucose = new byte[2];
                            glucose[0] = data[0x07];
                            glucose[1] = data[0x08];
                            glucose[1] <<= 4;
                            glucose[1] >>= 4;

                            //parse date
                            byte[] recordDate = new byte[4];
                            Array.Copy(data, 4, recordDate, 0, 3);
                            DateTime recordDateTime = DateTime.Parse("1/1/2000 00:00");
                            int minutesFromSpike = BitConverter.ToInt32(recordDate, 0);
                            recordDateTime = recordDateTime.AddMinutes((double)minutesFromSpike);

                            try
                            {
                                OnRecordRead(new RecordReadEventArgs(Records.AddRecordRow(recordDateTime, BitConverter.ToInt16(glucose, 0), (SampleFormat == SampleFormat.MGDL ? "mg/dl" : "mmol"))));
                            }
                            catch { }
                        }
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
            _MeterFound = false;

            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);

            //wait till the meter responds once before proceeding
            while (!_MeterFound)
            {
                try
                {
                    SendCommand("ack");
                }//try
                catch { }

                System.Threading.Thread.Sleep(1000);
            }//while

            SendCommand("serial");
            SendCommand("soft");
            SendCommand("format");
            SendCommand("ack");
            SendCommand("ack");
            
            SampleCount = 5000;

            _EndRecordEncountered = false;
            for (short i = 1; i < 500; i++)
            {
                recordIndex = i;
                //keep sending requests till the end record is encountered
                if (!_EndRecordEncountered)
                    SendCommand("read", i);
            }//for

            Thread.Sleep(1000);
        }

        private void SendCommand(string cmd)
        {
            SendCommand(cmd, 0);
        }
        private void SendCommand(string cmd, short arg)
        {
            //wait till another command finishes
            while (currentCommand != String.Empty)
            {
                if (cmd.ToLowerInvariant() == "test")
                {
                    currentCommand = string.Empty;
                    break;
                }

                if (currentCommand == "ack" && (DateTime.Now - AckSentTime) > TimeSpan.FromSeconds(3))
                    currentCommand = string.Empty;

                Thread.Sleep(10);
            }

            RawData = String.Empty;

            byte[] command = new byte[1];
            currentCommand = cmd;  
          
            switch (cmd)
            {
                case "test":
                case "ack": //contact the meter for a known response
                    command = new byte[] { 0x11, 0x0d, 0x44, 0x4D, 0x53, 0x0D, 0x0D };
                    AckSentTime = DateTime.Now;
                    break;
                case "serial": //read the serial number
                    command = new byte[] { 0x11, 0x0d, (byte)'D', (byte)'M', (byte)'@', 0x0D, 0x0D };
                    break;
                case "soft": //read the software version and date
                    command = new byte[] { 0x11, 0x0d, (byte)'D', (byte)'M', (byte)'?', 0x0D, 0x0D };
                    break;
                case "format": //read the software version and date
                    command = new byte[] { 0x11, 0x0d, (byte)'D', (byte)'M', (byte)'S', (byte)'U', (byte)'?', 0x0D, 0x0D };
                    break;
                case "read": //read the glucose sample records
                    byte[] prefix = new byte[] { 0x10, 0x02 };
                    byte[] suffix = new byte[] { 0x10, 0x03 };

                    List<byte> shortcommand = new List<byte>();
                    shortcommand.AddRange(new byte[] { 0x48, 0x52 });
                    byte [] recnum = BitConverter.GetBytes(arg);
                    Array.Reverse(recnum);
                    shortcommand.AddRange(recnum);

                    //calculate the checksum
                    byte checksum = calcCheckSumUS(shortcommand.ToArray());

                    List<byte> fullcommand = new List<byte>();
                    fullcommand.AddRange(prefix);
                    fullcommand.AddRange(escape10s(shortcommand).ToArray());
                    fullcommand.AddRange(suffix);
                    fullcommand.Add(checksum);

                    command = fullcommand.ToArray();

                    break;
                case "count": //get count of records in meter
                    byte[] cprefix = new byte[] { 0x10, 0x02 };
                    byte[] csuffix = new byte[] { 0x10, 0x03 };

                    List<byte> cshortcommand = new List<byte>();
                    cshortcommand.AddRange(new byte[] { 0x48, 0x52, 0xF5, 0x01 });
                    
                    //calculate the checksum
                    byte cchecksum = calcCheckSumUS(cshortcommand.ToArray());

                    List<byte> cfullcommand = new List<byte>();
                    cfullcommand.AddRange(cprefix);
                    cfullcommand.AddRange(escape10s(cshortcommand).ToArray());
                    cfullcommand.AddRange(csuffix);
                    cfullcommand.Add(cchecksum);

                    command = cfullcommand.ToArray();
                    break;
                default: 
                    break;
            }

            if (Port.IsOpen)
                try
                {
                    Port.Write(command, 0, command.Length);
                }
                catch { }
        }
        private readonly byte[] _lookupCRC = new byte[] { 0x00, 0x1a, 0x34, 0x2e, 0x68, 0x72, 0x5c, 0x46, 0x5d, 0x47, 0x69, 0x73, 0x35, 0x2f, 0x01, 0x1b, 0x37, 0x2d, 0x03, 0x19, 0x5f, 0x45, 0x6b, 0x71, 0x6a, 0x70, 0x5e, 0x44, 0x02, 0x18, 0x36, 0x2c, 0x6e, 0x74, 0x5a, 0x40, 0x06, 0x1c, 0x32, 0x28, 0x33, 0x29, 0x07, 0x1d, 0x5b, 0x41, 0x6f, 0x75, 0x59, 0x43, 0x6d, 0x77, 0x31, 0x2b, 0x05, 0x1f, 0x04, 0x1e, 0x30, 0x2a, 0x6c, 0x76, 0x58, 0x42, 0x51, 0x4b, 0x65, 0x7f, 0x39, 0x23, 0x0d, 0x17, 0x0c, 0x16, 0x38, 0x22, 0x64, 0x7e, 0x50, 0x4a, 0x66, 0x7c, 0x52, 0x48, 0x0e, 0x14, 0x3a, 0x20, 0x3b, 0x21, 0x0f, 0x15, 0x53, 0x49, 0x67, 0x7d, 0x3f, 0x25, 0x0b, 0x11, 0x57, 0x4d, 0x63, 0x79, 0x62, 0x78, 0x56, 0x4c, 0x0a, 0x10, 0x3e, 0x24, 0x08, 0x12, 0x3c, 0x26, 0x60, 0x7a, 0x54, 0x4e, 0x55, 0x4f, 0x61, 0x7b, 0x3d, 0x27, 0x09, 0x13, 0x2f, 0x35, 0x1b, 0x01, 0x47, 0x5d, 0x73, 0x69, 0x72, 0x68, 0x46, 0x5c, 0x1a, 0x00, 0x2e, 0x34, 0x18, 0x02, 0x2c, 0x36, 0x70, 0x6a, 0x44, 0x5e, 0x45, 0x5f, 0x71, 0x6b, 0x2d, 0x37, 0x19, 0x03, 0x41, 0x5b, 0x75, 0x6f, 0x29, 0x33, 0x1d, 0x07, 0x1c, 0x06, 0x28, 0x32, 0x74, 0x6e, 0x40, 0x5a, 0x76, 0x6c, 0x42, 0x58, 0x1e, 0x04, 0x2a, 0x30, 0x2b, 0x31, 0x1f, 0x05, 0x43, 0x59, 0x77, 0x6d, 0x7e, 0x64, 0x4a, 0x50, 0x16, 0x0c, 0x22, 0x38, 0x23, 0x39, 0x17, 0x0d, 0x4b, 0x51, 0x7f, 0x65, 0x49, 0x53, 0x7d, 0x67, 0x21, 0x3b, 0x15, 0x0f, 0x14, 0x0e, 0x20, 0x3a, 0x7c, 0x66, 0x48, 0x52, 0x10, 0x0a, 0x24, 0x3e, 0x78, 0x62, 0x4c, 0x56, 0x4d, 0x57, 0x79, 0x63, 0x25, 0x3f, 0x11, 0x0b, 0x27, 0x3d, 0x13, 0x09, 0x4f, 0x55, 0x7b, 0x61, 0x7a, 0x60, 0x4e, 0x54, 0x12, 0x08, 0x26, 0x3C };
        private byte calcCheckSumUS(byte[] Packet)
        {
            int crc = 0x7F;

            foreach (byte dataByte in Packet)
            {
                crc = _lookupCRC[crc & 0xFF];
                crc ^= dataByte;
            }

            crc = _lookupCRC[crc];
            crc ^= 0;

            return (byte)crc;
        }
        private List<byte> escape10s(List<byte> aPacket)
        {
            List<byte> escapedPacket = new List<byte>();
            for (int pos = 0; pos < aPacket.Count; pos++)
            {
                escapedPacket.Add(aPacket[pos]);
                if (aPacket[pos] == 0x10)
                {
                    escapedPacket.Add(0x10);
                }
            }
            return escapedPacket;
        }
        private byte[] unEscape10s(byte[] aPacket)
        {
            List<byte> cleanedPacket = new List<byte>(12);
            for (int pos = 0; pos < aPacket.Length; pos++)
            {
                cleanedPacket.Add(aPacket[pos]);
                if (pos < aPacket.GetUpperBound(0))
                {
                    if (aPacket.Length > pos + 1 && aPacket[pos] == 0x10 &&
                        aPacket[pos + 1] == 0x10)
                    {
                        pos++;
                    }
                }
            }

            return cleanedPacket.ToArray();
        }
    }
}
