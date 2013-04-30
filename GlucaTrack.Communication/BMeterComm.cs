using System;
using System.Collections.Generic;
using System.Text;

namespace GlucaTrack.Communication
{
    /// <summary>
    /// Ascii code representations of the communication requirements for some meter protocols.
    /// </summary>
    public enum AsciiCodes : byte { STX = 02, ETX = 03, EOT = 04, ENQ = 05, ACK = 06, LF = 10, CR = 13, ETB = 23}
    
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
        public ReadFinishedEventArgs(Records.RecordDataTable rows)
        {
            Rows = rows;
        }

        public Records.RecordDataTable Rows
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
        public HeaderReadEventArgs(int rowcount)
        {
            RowCount = rowcount;
        }

        public int RowCount
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
