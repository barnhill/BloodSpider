using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Data;
using System.Threading;
using GlucaTrack.Communication;

namespace GlucaTrack.Communication
{
    public abstract class AbstractMeter : IDisposable
    {
        #region Private Variables
        private SerialPort _port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        private DateTime _startReadTime = new DateTime();
        private TimeSpan _ReadTime = new TimeSpan();
        public Records.RecordDataTable _Records = new Records.RecordDataTable();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the serial port for communication.
        /// </summary>
        public SerialPort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Lookup id into the MeterTypes table
        /// </summary>
        public virtual int ID
        {
            get;
            set;
        }

        public virtual string SerialNumber
        {
            get;
            set;
        }

        public virtual string MeterDescription
        {
            get;
            set;
        }

        public virtual int SampleCount
        {
            get;
            set;
        }

        public virtual SampleFormat SampleFormat
        {
            get;
            set;
        }

        public Records.RecordDataTable Records
        {
            get
            {
                return _Records;
            }
            set
            {
                _Records = value;
            }
        }

        public virtual string RawData
        {
            get;
            set;
        }

        public virtual double ReadTime
        {
            get
            {
                return Math.Round(_ReadTime.TotalSeconds, 2);
            }
        }

        public Dictionary<string, string> MeterTypes
        {
            get { return Statics.MeterToClassLookup; }
        }

        public bool IsPortOpen
        {
            get { return Port.IsOpen; }
        }
        #endregion

        #region Events
        public event EventHandler ReadFinished;
        public event EventHandler RecordRead;
        public event EventHandler HeaderRead;
        public event EventHandler ErrorHandler;
        #endregion

        #region Event Handlers (Protected)
        protected virtual void OnReadFinished(ReadFinishedEventArgs e)
        {
            _ReadTime = DateTime.Now - _startReadTime;
            EventHandler invoker = ReadFinished;

            if (invoker != null) 
                invoker(this, e);
        }
        protected virtual void OnRecordRead(RecordReadEventArgs e)
        {
            EventHandler invoker = RecordRead;

            if (invoker != null) 
                invoker(this, e);
        }
        protected virtual void OnHeaderRead(HeaderReadEventArgs e)
        {
            EventHandler invoker = HeaderRead;
            _startReadTime = DateTime.Now;

            if (invoker != null)
                invoker(this, e);
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            EventHandler invoker = ErrorHandler;

            if (invoker != null)
                invoker(this, e);
        }
        #endregion

        #region Methods

        public AbstractMeter()
        {
        }

        /// <summary>
        /// Closes the serial port.
        /// </summary>
        /// <returns>Serial port closed?</returns>
        public virtual bool Close()
        {
            Port.DataReceived += null;
            
            if (Port.IsOpen)
            {
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
                Port.Close();
            }

            return !Port.IsOpen;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Port.IsOpen)
                {
                    Port.DiscardInBuffer();
                    Port.DiscardOutBuffer();
                    Close();
                }//if

                _port.Dispose();

                _Records.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Clears the buffers, closes the port and disposes of the port object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
