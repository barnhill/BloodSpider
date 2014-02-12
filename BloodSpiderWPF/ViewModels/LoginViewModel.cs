using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using GlucaTrackWPF.Models;

namespace GlucaTrackWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private LoginModel _login = new LoginModel();

        /// <summary>
        /// Shows or hides the error message on the login screen.
        /// </summary>
        public bool IsErrorVisible
        {
            get 
            {
                return !string.IsNullOrEmpty(Login.ErrorMessage);
            }
        }

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

        public LoginViewModel()
        {
            Login = new LoginModel();
        }

        public bool PerformLogin()
        {
            //verify login information
            try
            {
                
                string temp = SecureStringToString(Login.Password);
                Login.GetUserInfo(Login.LoginName, temp);
            }
            catch (ApplicationException aex)
            {
                Login.SessionId = Guid.Empty;
                Login.ErrorMessage = aex.Message;
                NotifyPropertyChanged("IsErrorVisible");
            }

            return Login.SessionId != Guid.Empty;
        }

        public static String SecureStringToString(System.Security.SecureString input)
        {
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            string s = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            return s;
        }
    }
}
