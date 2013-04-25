using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.ServiceModel;
using GlucaTrack.Communication;

namespace GlucaTrackWPF.Models
{
    public class MainModel : ModelBase
    {
        private bool _foundMeter = false;
        private DeviceInfo _device = new DeviceInfo();
        private LoginModel _login = new LoginModel();

        public LoginModel Login
        {
            get
            {
                return _login;
            }
            set
            {
                if (_login != value)
                {
                    _login = value;
                    NotifyPropertyChanged("Login");
                }
            }
        }

        public DeviceInfo Device
        {
            get
            {
                return _device;
            }
            set
            {
                if (_device != value)
                {
                    _device = value;
                    NotifyPropertyChanged("Device");
                }
            }
        }

        public bool IsMeterNotFound
        {
            get 
            {
                return !_foundMeter;
            }
        }

        public bool IsMeterFound
        {
            get
            {
                return _foundMeter;
            }
            set
            {
                if (_foundMeter != value)
                {
                    _foundMeter = value;
                    NotifyPropertyChanged("IsMeterFound");
                    NotifyPropertyChanged("IsMeterNotFound");
                }
            }
        }
    }
}
