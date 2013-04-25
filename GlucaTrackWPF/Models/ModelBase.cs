using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GlucaTrackWPF.Models
{
    public class ModelBase : INotifyPropertyChanged
    {
        private Guid _sessionId = Guid.Empty;

        /// <summary>
        /// Session id once authenticated.
        /// </summary>
        public Guid SessionId
        {
            get
            {
                return _sessionId;
            }
            set
            {
                if (value != _sessionId)
                {
                    _sessionId = value;
                    NotifyPropertyChanged("SessionId");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify of Property Changed event
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
