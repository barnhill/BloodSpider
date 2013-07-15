using System;
using System.Collections.Generic;
using System.Text;

namespace GlucaTrack.Communication
{
    /// <summary>
    /// Ascii code representations of the communication requirements for some meter protocols.
    /// </summary>
    public enum AsciiCodes : byte { NUL = 0x00, RH = 0x01, STX = 0x02, ETX = 0x03, EOT = 0x04, ENQ = 0x05, ACK = 0x06, LF = 0x0A, CR = 0x0D, NAK = 0x15, ETB = 0x17 }
    
    /// <summary>
    /// Sample record format
    /// </summary>
    public enum SampleFormat : byte { MGDL = 00, MMOL = 01 };
    
    /// <summary>
    /// Event arguments when a record is read from a meter.
    /// </summary>
    public class RecordReadEventArgs : EventArgs
    {
        public RecordReadEventArgs(Records.RecordRow _row)
        {
            Row = _row;
        }

        public Records.RecordRow Row
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Event arguments when a record is read from a meter.
    /// </summary>
    public class ReadFinishedEventArgs : EventArgs
    {
        public ReadFinishedEventArgs(object meter)
        {
            Meter = meter;
        }

        public object Meter
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Event arguments when a header record is read from a meter.
    /// </summary>
    public class HeaderReadEventArgs : EventArgs
    {
        public HeaderReadEventArgs(int rowcount, object meter)
        {
            RowCount = rowcount;
            Meter = meter;
        }

        public int RowCount
        {
            get;
            set;
        }

        public object Meter
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Event arguments when a error is encountered.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(Exception ex)
        {
            Error = ex;
        }

        public Exception Error
        {
            get;
            set;
        }
    }
}
