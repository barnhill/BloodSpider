using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters
{
    public class Contour_USB : AbstractMeter, IMeterHID
    {
        int _vid = int.MinValue;
        int _pid = int.MinValue;
        byte[] writeBuffer = new byte[65];
        byte[] readBuffer = new byte[40];
        bool _devicePresent = false;
        byte _CountStep = 0;
        Dictionary<int, string> _supportedPIDs = new Dictionary<int, string>();

        bool _PreHeaderRead = false;
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

        public Contour_USB()
        {
            ID = 8;
            VID = Int32.Parse("1A79", System.Globalization.NumberStyles.HexNumber);

            //load supported pids
            PopulateSupportedPIDs();
            
            Port = new UsbLibrary.UsbHidPort();

            Port.OnSpecifiedDeviceRemoved += Port_OnSpecifiedDeviceRemoved;
            Port.OnSpecifiedDeviceArrived += Port_OnSpecifiedDeviceArrived;
        }

        private void PopulateSupportedPIDs()
        {
            SupportedPIDs.Add(Int32.Parse("6002", System.Globalization.NumberStyles.HexNumber), "Bayer Contour USB");
            SupportedPIDs.Add(Int32.Parse("7410", System.Globalization.NumberStyles.HexNumber), "Bayer Contour USB Next");
        }

        public void ReadData()
        {
            if (Port == null || Port.SpecifiedDevice == null || !_devicePresent) 
                return;

            writeBuffer[3] = 0x01;
            writeBuffer[4] = 0x05;

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
        }

        public bool Connect()
        {
            if (VID == int.MinValue)
                throw new ArgumentNullException("VID is blank or null");

            return IsMeterConnected();
        }

        public bool IsMeterConnected()
        {
            if (VID == int.MinValue)
                throw new ArgumentNullException("VID is blank or null");

            foreach (KeyValuePair<int, string> item in SupportedPIDs)
            {
                PID = item.Key;
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
            char[] asciiChars = new char[Encoding.ASCII.GetCharCount(e.data, 0, e.data.Length)];
            Encoding.ASCII.GetChars(e.data, 0, e.data.Length, asciiChars, 0);
            string newString = new string(asciiChars);
            tempString += new string(asciiChars).Substring(6);

            if (_HeaderRead && !_NumResultsRead && (tempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ENQ)) || tempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ACK))))
            {
                return;
            }

            //preheader (NUL + NUL)
            string preHeaderCompare = ((char)AsciiCodes.NUL).ToString() + ((char)AsciiCodes.NUL).ToString();
            if (tempString.Length > 5 && tempString.StartsWith(preHeaderCompare))
            {
                _PreHeaderRead = true;
                tempString = string.Empty;
                writeBuffer[3] = 0x01;
                writeBuffer[4] = (byte)AsciiCodes.ACK;
                Port.SpecifiedDevice.SendData(writeBuffer);
                return;
            }

            if (tempString.Contains("\r\n"))
            {
                tempString = tempString.Split(new string [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)[0];
                
                //header
                if (!_HeaderRead && _PreHeaderRead && tempString.StartsWith(((char)AsciiCodes.STX).ToString() + "1H"))
                {
                    _HeaderRead = true;
                    ParseHeader(tempString);
                    tempString = string.Empty;
                    writeBuffer[3] = 0x00;
                    writeBuffer[4] = (byte)AsciiCodes.ACK;
                    Port.SpecifiedDevice.SendData(writeBuffer);
                    return;
                }
            }
        }
        
        private void ParseHeader (string header)
        {
            string[] headerrecord = header.Split(new char[] { '|' });
            string[] typeandserial = headerrecord[4].Split(new char[] { '^' });

            string accesspassword = headerrecord[3];
            string softwareversion = typeandserial[1].Split(new char[] { '\\' })[0];
            string eepromversion = typeandserial[1].Split(new char[] { '\\' })[1];
            MeterDescription = typeandserial[0];
            
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
    }
}
