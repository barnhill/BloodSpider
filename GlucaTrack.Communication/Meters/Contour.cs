using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GlucaTrack.Communication.Meters.Bayer
{
    public class Contour: AbstractMeter, IMeter
    {
        string _TempString = String.Empty;
        bool _HeaderRead = false;
        bool _NumResultsRead = false;
        bool _ConfigRead = false;
        byte _CountStep = 0;
        bool _TestMode = false;
        bool _TestFailed = false;
        bool _MeterFound = false;

        public Contour()
        {
            ID = 2;
            MeterDescription = "Bayer Contour";
        }

        public bool Open()
        {
            Port.DtrEnable = true;

            try
            {
                if (!Port.IsOpen)
                {
                    Thread.Sleep(250);
                    Port.Open();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Port.IsOpen;
            }

            Thread.Sleep(250);

            //clear the buffers
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
            Port.BaseStream.Flush();

            return Port.IsOpen;
        }

        public bool Connect(string COMport)
        {
            if (Port != null)
            {
                Dispose();
            }

            Port = new SerialPort(COMport, 9600, Parity.None, 8, StopBits.One);
            Port.ReadBufferSize = 8096;

            return Open();
        }

        public bool IsMeterConnected(string COMport)
        {
            Connect(COMport);

            if (!Port.IsOpen)
                return false;

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            _HeaderRead = false;
            _NumResultsRead = false;
            _MeterFound = false;
            _TestFailed = false;
            _TestMode = true;

            Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);

            DateTime dtStartLoop = DateTime.Now;
            while (!_TestFailed && !_MeterFound && (DateTime.Now - dtStartLoop).TotalMilliseconds < 20000)
            {
                Thread.Sleep(100);
            }

            base.Close();

            _TestMode = false;

            return _MeterFound;
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _TempString += Port.ReadExisting();

            //Get number of samples on meter
            if (!_NumResultsRead && (_TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ENQ)) || _TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ACK))))
            {
                switch (_CountStep)
                {
                    case 0: Port.Write(new byte[] { 0x15 }, 0, 1);
                        _CountStep++;
                        break;
                    case 1: Port.Write(new byte[] { 0x05 }, 0, 1);
                        _CountStep++;
                        break;
                    case 2: Port.Write("R|");
                        _CountStep++;
                        break;
                    case 3: Port.Write("M|");
                        _CountStep++;
                        break;
                    case 4: _NumResultsRead = true;
                        try
                        {
                            string tempCount = _TempString.Substring(_TempString.IndexOf("|") + 1, _TempString.LastIndexOf("|") - _TempString.IndexOf("|") - 1);
                            SampleCount = Convert.ToInt32(tempCount);
                            Port.Write(new byte[] { 0x04 }, 0, 1);
                            _CountStep = 0;
                            _TempString = string.Empty;
                        }
                        catch
                        {
                            Port.DataReceived += null;
                            Port.DiscardInBuffer();
                            Port.DiscardOutBuffer();
                            _TestFailed = true;
                            return;
                        }

#if DEBUG
                            Console.WriteLine("SampleCount: " + SampleCount);                           
#endif
                        break;
                    default: break;
                }//switch
                
                return;
            }//if
            else if (_NumResultsRead && !_ConfigRead && (_TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ENQ)) || _TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ACK))))
            {
                switch (_CountStep)
                {
                    case 0: Port.Write(new byte[] { 0x15 }, 0, 1);
                        _CountStep++;
                        break;
                    case 1: Port.Write(new byte[] { 0x05 }, 0, 1);
                        _CountStep++;
                        break;
                    case 2: Port.Write("R|");
                        _CountStep++;
                        break;
                    case 3: Port.Write("C|");
                        _CountStep++;
                        break;
                    case 4: _ConfigRead = true;
                        try
                        {
                            string[] splitData = _TempString.Split(new char[] { '|' });
                            System.Collections.BitArray bitary = new System.Collections.BitArray(Byte.Parse(splitData[1]));
                            SampleFormat = (bitary.Get(2)) ? SampleFormat.MMOL : SampleFormat.MGDL;
                            Port.Write(new byte[] { 0x04 }, 0, 1);
                            _CountStep = 0;
                            _TempString = string.Empty;
                        }
                        catch 
                        {
                            Port.DataReceived += null;
                            Port.DiscardInBuffer();
                            Port.DiscardOutBuffer();
                            _TestFailed = true;
                            return;
                        }
                        
#if DEBUG
                        Console.WriteLine("SampleFormat: " + SampleFormat.ToString());
#endif
                        break;
                    default: break;
                }//switch

                return;
            }//else
            //if data received is the ENQ to start communications
            else if (!_HeaderRead && _TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ENQ)))
            {
                Port.Write(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ACK));
                
                RawData += _TempString;
                _TempString = String.Empty;

                return;
            }//if

            //if data contains an STX and a following LF then a full frame can be trimmed
            else if (_TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.STX)) && _TempString.Substring(_TempString.IndexOf(Statics.GetStringFromAsciiCode((byte)AsciiCodes.STX))).Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.LF)))
            {
                RawData += _TempString;

                //full frame encountered (cut out full frame, and remove the STX on the front)
                string fullframe = _TempString.Split(new string[] { Statics.GetStringFromAsciiCode((byte)AsciiCodes.CR) }, StringSplitOptions.None)[0].Replace(Statics.GetStringFromAsciiCode((byte)AsciiCodes.STX), "");
                
                //trim off the frame as more data may be in the buffer
                _TempString = _TempString.Substring(_TempString.LastIndexOf(Statics.GetStringFromAsciiCode((byte)AsciiCodes.LF)) + 1);
                
                #region HeaderRecord
                if (fullframe[1] == 'H')
                {
                    _HeaderRead = true;
                    string[] headerrecord = fullframe.Split(new char[] { '|' });
                    string[] typeandserial = headerrecord[4].Split(new char[] { '^' });

                    string accesspassword = headerrecord[3];
                    string softwareversion = typeandserial[1].Split(new char[] { '\\' })[0];
                    string eepromversion = typeandserial[1].Split(new char[] { '\\' })[1];
                    MeterDescription = typeandserial[0];

                    string MeterType = SplitTypeandSerial(typeandserial[2])[0];
                    SerialNumber = SplitTypeandSerial(typeandserial[2])[1];

                    //contour meters have a product number of 7150
                    _MeterFound = (MeterType == "7150");

                    if (_TestMode)
                    {
                        Port.DataReceived += null;
                        return;
                    }

                    OnHeaderRead(new HeaderReadEventArgs(SampleCount, this));

                    Console.WriteLine("Header: " + fullframe);
                }//if
                #endregion

                #region Glucose Record
                else if (fullframe[1] == 'R')
                {
                    string[] splitrecord = fullframe.Split(new char[] { '|' });

                    //only if glucose record
                    if (splitrecord.Length > 10)
                    {
                        int year = int.Parse(splitrecord[11].Substring(0, 4));
                        int month = int.Parse(splitrecord[11].Substring(4, 2));
                        int day = int.Parse(splitrecord[11].Substring(6, 2));
                        int hour = int.Parse(splitrecord[11].Substring(8, 2));
                        int minute = int.Parse(splitrecord[11].Substring(10, 2));
                        int glucose = int.Parse(splitrecord[3]);
                        string units = splitrecord[4].Split(new char[] { '^' })[0];

                        DateTime dtTimeStamp = new DateTime(year, month, day, hour, minute, 0);

                        //put the record in the dataset and raise the read event
                        try
                        {
                            if (Records.FindByTimestamp(dtTimeStamp) == null)
                            {
#if DEBUG
                                Console.WriteLine("Record: " + fullframe);
#endif
                                OnRecordRead(new RecordReadEventArgs(this._Records.AddRecordRow(dtTimeStamp, glucose, units)));
                            }//if
                            else
                            {
#if DEBUG
                                Console.WriteLine("DUPLIC: " + fullframe);
#endif
                            }//else
                        }//try
                        catch
                        { 
                        }//catch
                    }//if
                }//elseif
                #endregion
            }//else
            
            //end of transmission encountered after a header record is read
            else if (_HeaderRead && _TempString.Contains(Statics.GetStringFromAsciiCode((byte)AsciiCodes.EOT)))
            {
                _HeaderRead = false;
                Port.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
                OnReadFinished(new ReadFinishedEventArgs(this));
                Close();
                Dispose();
                return;
            }//elseif

            //send response
            if (_NumResultsRead)
                Port.Write(Statics.GetStringFromAsciiCode((byte)AsciiCodes.ACK));
        }

        public void ReadData()
        {
            if (!Port.IsOpen)
                throw new Exception("Port is closed.");

            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
            _HeaderRead = false;
            Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
        }
        
        private string[] SplitTypeandSerial(string raw)
        {
            int temp = 0;
            int breakIndex = 0;
            for(int i = 0; i < raw.Length; i++)
            {
                if (!int.TryParse(raw[i].ToString(), out temp))
                {
                    breakIndex = i;
                }
            }

            return raw.Split(raw[breakIndex]);
        }
    }
}
