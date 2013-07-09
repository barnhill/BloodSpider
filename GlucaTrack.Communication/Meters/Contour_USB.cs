using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters
{
    class Contour_USB : AbstractMeter, IMeterHID
    {
        int _vid = int.MinValue;
        int _pid = int.MinValue;

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

        public Contour_USB()
        {
            ID = 8;
            VID = Int32.Parse("1A79", System.Globalization.NumberStyles.HexNumber);
            PID = Int32.Parse("6002", System.Globalization.NumberStyles.HexNumber);
            Port = new UsbLibrary.UsbHidPort();
            Port.VendorId = VID;
            Port.ProductId = PID;
            MeterDescription = "Bayer Contour USB";
        }

        public void ReadData()
        {
            throw new NotImplementedException();
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            if (VID == int.MinValue)
                throw new ArgumentNullException("VID is blank or null");
            if (PID == int.MinValue)
                throw new ArgumentNullException("PID is blank or null");

            return true;
        }

        public bool IsMeterConnected()
        {
            if (VID == int.MinValue)
                throw new ArgumentNullException("VID is blank or null");
            if (PID == int.MinValue)
                throw new ArgumentNullException("PID is blank or null");

            return Port.IsDevicePresent();
        }
    }
}
