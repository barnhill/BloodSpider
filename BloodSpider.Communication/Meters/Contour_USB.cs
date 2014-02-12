using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace BloodSpider.Communication.Meters
{
    public class Contour_USB : AbstractMeter, IMeterHID
    {
        int _vid = int.MinValue;
        int _pid = int.MinValue;
        static byte[] writeBuffer = new byte[65];
        byte[] readBuffer = new byte[40];
        bool _devicePresent = false;
        byte _CountStep = 0;
        Dictionary<int, string> _supportedPIDs = new Dictionary<int, string>();
        Dictionary<int, int> _supportedIDs = new Dictionary<int, int>();
        int RecordsRead;

        bool _HeaderRead = false;
        bool _NumResultsRead = false;

        string tempString = string.Empty;

        public new UsbLibrary.UsbHidPort Port
        {
            get;
            set;
        }

        public int VID
        {
            get
            {
                return _vid;
            }
            set
            {
                _vid = value;
            }
        }

        public int PID
        {
            get
            {
                return _pid;
            }
            set
            {
                _pid = value;
            }
        }

        /// <summary>
        /// List of supported PIDs
        /// </summary>
        public Dictionary<int, string> SupportedPIDs
        {
            get
            {
                return _supportedPIDs;
            }
        }

        /// <summary>
        /// List of supported IDs (table MeterTypes.metertype_id)
        /// </summary>
        public Dictionary<int, int> SupportedIDs
        {
            get
            {
                return _supportedIDs;
            }
        }

        public Contour_USB()
        {
            ID = 8;
            VID = Int32.Parse("1A79", System.Globalization.NumberStyles.HexNumber);
            MeterDescription = "Bayer Contour USB";

            //load supported pids
            PopulateSupportedPIDs();

            Port = new UsbLibrary.UsbHidPort();

            Port.OnSpecifiedDeviceRemoved += Port_OnSpecifiedDeviceRemoved;
            Port.OnSpecifiedDeviceArrived += Port_OnSpecifiedDeviceArrived;
        }

        private void PopulateSupportedPIDs()
        {
            SupportedPIDs.Add(0x6002, "Bayer Contour USB");
            SupportedPIDs.Add(0x7410, "Bayer Contour USB Next");

            SupportedIDs.Add(0x6002, 8);
            SupportedIDs.Add(0x7410, 9);
        }

        public void ReadData()
        {
            if (Port == null || Port.SpecifiedDevice == null || !_devicePresent)
                return;
            
            RecordsRead = 0;

            writeBuffer[3] = (byte)AsciiCodes.RH;
            writeBuffer[4] = (byte)AsciiCodes.ACK;

            Port.SpecifiedDevice.DataRecieved += SpecifiedDevice_DataRecieved;

            //request header and start the request for all data
            Port.SpecifiedDevice.SendData(writeBuffer);
        }

        void SpecifiedDevice_DataRecieved(object sender, UsbLibrary.DataRecievedEventArgs args)
        {
            DataReceived(sender, args);
        }

        void Port_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            _devicePresent = true;
        }

        void Port_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            _devicePresent = false;
            Port.OnDataRecieved -= new UsbLibrary.DataRecievedEventHandler(DataReceived);
        }

        public bool Connect()
        {
            throw new NotImplementedException("Method not implemented due to HID not requiring connection.");
        }

        public bool IsMeterConnected()
        {
            if (VID == int.MinValue)
                throw new ArgumentNullException("VID is blank or null");

            foreach (KeyValuePair<int, string> item in SupportedPIDs)
            {
                PID = item.Key;
                ID = SupportedIDs[item.Key];
                MeterDescription = item.Value;

                Port.VendorId = VID;
                Port.ProductId = PID;
                Port.CheckDevicePresent();

                if (_devicePresent)
                    break;
                else
                    PID = int.MinValue;
            }

            return _devicePresent;
        }

        public void DataReceived(object sender, UsbLibrary.DataRecievedEventArgs e)
        {
            Array.Clear(writeBuffer, 0, writeBuffer.Length);
            char[] asciiChars = new char[Encoding.ASCII.GetCharCount(e.data, 0, e.data.Length)];
            Encoding.ASCII.GetChars(e.data, 0, e.data.Length, asciiChars, 0);
            string newString = new string(asciiChars);
            tempString += new string(asciiChars).Substring(5);
            tempString = tempString.Replace("\0", string.Empty);

            //send read number of results on device message
            if (_HeaderRead && !_NumResultsRead && _CountStep <= 3)
            {
                GetSampleCountMessages();
                return;
            }

            ////send read config message
            //if (_HeaderRead && _NumResultsRead && !_ConfigRead && _CountStep <= 1)
            //{
            //    GetConfigMessages();
            //    return;
            //}

            //preheader
            if (tempString.StartsWith(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ENQ)) && tempString.Length == 1)
            {
                tempString = string.Empty;
                writeBuffer[3] = (byte)AsciiCodes.RH;
                writeBuffer[4] = (byte)AsciiCodes.ACK;
                Port.SpecifiedDevice.SendData(writeBuffer);
                return;
            }

            //new line detected so frame is complete
            if (tempString.Contains("\r\n"))
            {
                tempString = tempString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0];

                //header
                if (tempString.Contains(((char)AsciiCodes.STX).ToString() + "1H"))
                {
                    _HeaderRead = true;
                    ParseHeader(tempString);
                    tempString = string.Empty;
                    writeBuffer[3] = (byte)AsciiCodes.RH;
                    writeBuffer[4] = (byte)AsciiCodes.EOT;
                    Port.SpecifiedDevice.SendData(writeBuffer);
                    Thread.Sleep(100);
                    return;
                }

                //patient record
                if (tempString.Length > tempString.IndexOf((char)AsciiCodes.STX) + 2 && tempString[tempString.IndexOf((char)AsciiCodes.STX) + 2] == 'P')
                {
                    tempString = string.Empty;
                }

                //terminator record
                if (tempString.Length > tempString.IndexOf((char)AsciiCodes.STX) + 2 && tempString[tempString.IndexOf((char)AsciiCodes.STX) + 2] == 'L')
                {
                    tempString = string.Empty;
                }

                //glucose record
                if (tempString.Contains("^^^Glucose"))
                {
                    string[] splitrecord = tempString.Split(new string[] { "|", "\r", "^" }, StringSplitOptions.RemoveEmptyEntries);
                    int year = int.Parse(splitrecord[7].Substring(0, 4));
                    int month = int.Parse(splitrecord[7].Substring(4, 2));
                    int day = int.Parse(splitrecord[7].Substring(6, 2));
                    int hour = int.Parse(splitrecord[7].Substring(8, 2));
                    int minute = int.Parse(splitrecord[7].Substring(10, 2));

                    DateTime dtTimeStamp = new DateTime(year, month, day, hour, minute, 0);

                    int glucose = int.Parse(splitrecord[3]);
                    string sampleFormat = splitrecord[4];

                    this.SampleFormat = sampleFormat.ToLowerInvariant().Contains("mmol") ? SampleFormat.MMOL : SampleFormat.MGDL;

                    //put the record in the dataset and raise the read event
                    try
                    {
                        if (Records.FindByTimestamp(dtTimeStamp) == null)
                        {
                            //only insert non-duplicate records
                            OnRecordRead(new RecordReadEventArgs(this._Records.AddRecordRow(dtTimeStamp, glucose, sampleFormat)));
                            RecordsRead++;
                        }//if
                    }//try
                    catch
                    {
                    }//catch

                    tempString = string.Empty;
                }

                //num results message arrived
                if (tempString.Contains("D|") && !_NumResultsRead)
                { 
                    //cut message out of string
                    string message = tempString.Substring(tempString.IndexOf("D|"));
                    string [] splits = tempString.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    this.SampleCount = Convert.ToInt32(splits[1]);
                    tempString = string.Empty;
                    _NumResultsRead = true;
                    _CountStep = 0;
                    Thread.Sleep(100);
                    OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));
                    //GetConfigMessages();
                    return;
                }

                ////results format
                //if (tempString.Contains("D|") && !_ConfigRead)
                //{
                //    //cut message out of string
                //    string message = tempString.Substring(tempString.IndexOf("D|"));
                //    string[] splits = tempString.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                //    BitArray bitary = new BitArray(Byte.Parse(splits[1]));
                //    SampleFormat = (bitary.Get(2)) ? SampleFormat.MMOL : SampleFormat.MGDL;
                //    tempString = string.Empty;
                //    _ConfigRead = true;
                //    _CountStep = 0;
                //    Thread.Sleep(100);
                //    return;
                //}
            }

            //end of transmission encountered after a header record is read
            if (_HeaderRead && _NumResultsRead && RecordsRead >= SampleCount)
            {
                _HeaderRead = false;
                Port.OnDataRecieved -= new UsbLibrary.DataRecievedEventHandler(DataReceived);
                Port.SpecifiedDevice.DataRecieved -= SpecifiedDevice_DataRecieved;
                OnReadFinished(new ReadFinishedEventArgs(this));
                Close();
                Dispose();
                return;
            }//elseif

            if (_NumResultsRead)// && _ConfigRead)
            {
                Array.Clear(writeBuffer, 0, writeBuffer.Length);
                writeBuffer[4] = (byte)AsciiCodes.RH;
                writeBuffer[5] = (byte)AsciiCodes.ACK;
                Port.SpecifiedDevice.SendData(writeBuffer);
                Thread.Sleep(100);
            }
        }

        private void ParseHeader(string header)
        {
            string[] headerrecord = header.Split(new char[] { '|' });
            string[] typeandserial = headerrecord[4].Split(new char[] { '^' });

            string accesspassword = headerrecord[3];
            string softwareversion = typeandserial[1].Split(new char[] { '\\' })[0];
            string eepromversion = typeandserial[1].Split(new char[] { '\\' })[1];

            string MeterType = SplitTypeandSerial(typeandserial[2])[0];
            SerialNumber = SplitTypeandSerial(typeandserial[2])[1].Substring(0, 7);
        }

        private string[] SplitTypeandSerial(string raw)
        {
            int temp = 0;
            int breakIndex = 0;
            for (int i = 0; i < raw.Length; i++)
            {
                if (!int.TryParse(raw[i].ToString(), out temp))
                {
                    breakIndex = i;
                    break;
                }
            }

            return raw.Split(raw[breakIndex]);
        }

        private void GetSampleCountMessages()
        {
            switch (_CountStep)
            {
                case 0:
                    Thread.Sleep(10);
                    writeBuffer[4] = (byte)AsciiCodes.RH;
                    writeBuffer[5] = (byte)AsciiCodes.NAK;
                    Port.SpecifiedDevice.SendData(writeBuffer);
                    _CountStep++;
                    break;
                case 1:
                    Thread.Sleep(10);
                    writeBuffer[4] = (byte)AsciiCodes.RH;
                    writeBuffer[5] = (byte)AsciiCodes.ENQ;
                    Port.SpecifiedDevice.SendData(writeBuffer);
                    _CountStep++;
                    break;
                case 2:
                    Thread.Sleep(10);
                    writeBuffer[4] = (byte)AsciiCodes.STX;
                    writeBuffer[5] = 0x52; //R
                    writeBuffer[6] = 0x7C; //|
                    Port.SpecifiedDevice.SendData(writeBuffer);
                    _CountStep++;
                    break;
                case 3:
                    Thread.Sleep(10);
                    writeBuffer[4] = (byte)AsciiCodes.STX;
                    writeBuffer[5] = 0x4D; //M
                    writeBuffer[6] = 0x7C; //|
                    Port.SpecifiedDevice.SendData(writeBuffer);
                    _CountStep++;
                    break;
                default: break;
            }
        }

        //private void GetConfigMessages()
        //{
        //    switch (_CountStep)
        //    {
        //        case 0:
        //            Thread.Sleep(100);
        //            writeBuffer[4] = (byte)AsciiCodes.STX;
        //            writeBuffer[5] = 0x52; //R
        //            writeBuffer[6] = 0x7C; //|
        //            Port.SpecifiedDevice.SendData(writeBuffer);
        //            _CountStep++;
        //            break;
        //        case 1:
        //            Thread.Sleep(100);
        //            writeBuffer[4] = (byte)AsciiCodes.STX;
        //            writeBuffer[5] = 0x43; //C
        //            writeBuffer[6] = 0x7C; //|
        //            Port.SpecifiedDevice.SendData(writeBuffer);
        //            _CountStep++;
        //            break;
        //        default: break;
        //    }
        //}
    }
}
