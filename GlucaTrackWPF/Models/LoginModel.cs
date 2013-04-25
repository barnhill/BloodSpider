using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.ServiceModel;

namespace GlucaTrackWPF.Models
{
    public class LoginModel : ModelBase
    {
        private string _firstName = string.Empty;
        private string _middleName = string.Empty;
        private string _lastName = string.Empty;
        private string _address1 = string.Empty;
        private string _address2 = string.Empty;
        private string _city = string.Empty;
        private string _state = string.Empty;
        private string _zipcode = string.Empty;
        private string _loginName = string.Empty;
        private System.Security.SecureString _password = new System.Security.SecureString();
        private DateTime _lastsync = new DateTime();
        private DateTime _lastweblogin = new DateTime();
        private string _usertype = string.Empty;
        private Guid _sessionid = Guid.Empty;
        private string _errorMessage = string.Empty;

        /// <summary>
        /// Users first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                if (value != _firstName)
                {
                    _firstName = value;
                    NotifyPropertyChanged("FirstName");
                }
            }
        }

        /// <summary>
        /// Users middle name.
        /// </summary>
        public string MiddleName
        {
            get
            {
                return _middleName;
            }
            set
            {
                if (value != _middleName)
                {
                    _middleName = value;
                    NotifyPropertyChanged("MiddleName");
                }
            }
        }

        /// <summary>
        /// Users last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (value != _lastName)
                {
                    _lastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        /// <summary>
        /// Users address line 1.
        /// </summary>
        public string Address1
        {
            get
            {
                return _address1;
            }
            set
            {
                if (value != _address1)
                {
                    _address1 = value;
                    NotifyPropertyChanged("Address1");
                }
            }
        }

        /// <summary>
        /// Users address line 2.
        /// </summary>
        public string Address2
        {
            get
            {
                return _address2;
            }
            set
            {
                if (value != _address2)
                {
                    _address2 = value;
                    NotifyPropertyChanged("Address2");
                }
            }
        }

        /// <summary>
        /// Users city.
        /// </summary>
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if (value != _city)
                {
                    _city = value;
                    NotifyPropertyChanged("City");
                }
            }
        }

        /// <summary>
        /// Users state.
        /// </summary>
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        /// <summary>
        /// Users zip code.
        /// </summary>
        public string Zipcode
        {
            get
            {
                return _zipcode;
            }
            set
            {
                if (value != _zipcode)
                {
                    _zipcode = value;
                    NotifyPropertyChanged("Zipcode");
                }
            }
        }

        /// <summary>
        /// Users login name.
        /// </summary>
        public string LoginName
        {
            get
            {
                return _loginName;
            }
            set
            {
                if (value != _loginName)
                {
                    _loginName = value;
                    NotifyPropertyChanged("LoginName");
                }
            }
        }

        /// <summary>
        /// Users password.
        /// </summary>
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// Users last synchronization date/time.
        /// </summary>
        public DateTime LastSync
        {
            get
            {
                return _lastsync;
            }
            set
            {
                if (value != _lastsync)
                {
                    _lastsync = value;
                    NotifyPropertyChanged("LastSync");
                }
            }
        }

        /// <summary>
        /// Short date representation of the last synchronization date.
        /// </summary>
        public string LastSyncDateString
        {
            get
            {
                if (_lastsync != DateTime.MinValue)
                    return (DateTime.Now - _lastsync).TotalDays.ToString();
                else
                    return "0";
            }
        }

        /// <summary>
        /// Users last web login date/time.
        /// </summary>
        public DateTime LastWebLogin
        {
            get
            {
                return _lastweblogin;
            }
            set
            {
                if (value != _lastweblogin)
                {
                    _lastweblogin = value;
                    NotifyPropertyChanged("LastWebLogin");
                }
            }
        }

        /// <summary>
        /// Users account type.
        /// </summary>
        public string UserType
        {
            get
            {
                return _usertype;
            }
            set
            {
                if (value != _usertype)
                {
                    _usertype = value;
                    NotifyPropertyChanged("UserType");
                }
            }
        }

        /// <summary>
        /// Error message shown on the login screen.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (value != _errorMessage)
                {
                    _errorMessage = value;
                    NotifyPropertyChanged("ErrorMessage");
                    NotifyPropertyChanged("IsErrorVisible");
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginModel()
        {
        }

        /// <summary>
        /// Gets the users information from the WCF service if authentication passes.
        /// </summary>
        /// <param name="username">Username to try to authenticate.</param>
        /// <param name="password">Password to try to authenticate.</param>
        /// <returns>User demographical information</returns>
        public void GetUserInfo(string username, string password)
        {
            using (GlucaTrack.Services.GTServiceClient service = new GlucaTrack.Services.GTServiceClient())
            {
                service.Open();

                try
                {
                    if (service.State == CommunicationState.Opened)
                    {
                        //validate the users login and get the users information back
                        GlucaTrack.Services.DSUser userinfo = service.ValidateLogin(username, password);

                        //populate the login model with the info from the web service
                        GlucaTrack.Services.DSUser.sp_GetLoginRow loginrow = userinfo.sp_GetLogin.First();
                        this.FirstName = loginrow.firstname;
                        this.MiddleName = loginrow.middlename;
                        this.LastName = loginrow.lastname;
                        this.Address1 = loginrow.address1;
                        this.Address2 = loginrow.Isaddress2Null() ? string.Empty : loginrow.address2;
                        this.City = loginrow.city;
                        this.State = loginrow.state;
                        this.Zipcode = loginrow.zipcode;
                        this.LastSync = loginrow.Islast_syncNull() ? DateTime.MinValue : loginrow.last_sync;
                        this.LastWebLogin = loginrow.Islast_webloginNull() ? DateTime.MinValue : loginrow.last_weblogin;
                        this.UserType = loginrow.usertype;
                        this.SessionId = loginrow.sessionid;
                    }
                    else
                        throw new ApplicationException("Service is closed.");
                }
                catch(System.ServiceModel.FaultException fex)
                {
                    throw new ApplicationException(fex.Message);
                }
            }
        }
    }
}
