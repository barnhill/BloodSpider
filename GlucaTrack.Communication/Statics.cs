using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Reflection;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace GlucaTrack.Communication
{
    public static class Statics
    {
        public static string baseFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GlucaTrack");

        public static List<DeviceInfo> DevicesFound = new List<DeviceInfo>();

        public static Dictionary<string, string> MeterToClassLookup = new Dictionary<string, string>();

        /// <summary>
        /// Gets the string representation of an ascii code.
        /// </summary>
        /// <param name="convert">Ascii code to convert.</param>
        /// <returns></returns>
        internal static string GetStringFromAsciiCode(byte convert)
        {
            try
            {
                return Encoding.ASCII.GetChars(new byte[] { convert })[0].ToString();
            }//try
            catch
            {
                return String.Empty;
            }//catch
        }

        public static void PopulateMeterTypes()
        {
            MeterToClassLookup.Clear();

            MeterToClassLookup.Add("Freestyle", "Abbott.FreeStyle");

            MeterToClassLookup.Add("Contour", "Bayer.Contour");
            MeterToClassLookup.Add("Breeze2", "Bayer.Breeze2");

            MeterToClassLookup.Add("OneTouch Ultra 2", "LifeScan.OneTouch_Ultra2");
            MeterToClassLookup.Add("OneTouch Ultra Mini", "LifeScan.OneTouch_UltraMini");
            MeterToClassLookup.Add("OneTouch Ultra Smart", "LifeScan.OneTouch_UltraSmart");

            MeterToClassLookup.Add("Caresens N", "iSens.Caresens_N");
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        public static byte[] StrToByteArray(string str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }
        
        internal static void Error(string errorcode, Exception ex)
        {
        }

        public static string TrimNonPrintableCharacters(string s)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                byte b = (byte)c;
                if (b > 32)
                    result.Append(c);
            }
            return result.ToString();
        }

        public static List<string> GetComPortList()
        {
            List<string> listComPorts = new List<string>();
            foreach (string portname in SerialPort.GetPortNames())
            {
                listComPorts.Add(portname.Trim());
            }

            return listComPorts;
        }

        public static DeviceInfo DetectFirstDevice()
        {
            DevicesFound.Clear();

            List<BackgroundWorker> threads = new List<BackgroundWorker>();
            foreach (string comport in SerialPort.GetPortNames())
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(StubFindDevices);
                bgw.WorkerSupportsCancellation = true;
                bgw.RunWorkerAsync(comport);
                threads.Add(bgw);
                //StubFindDevices(null, new DoWorkEventArgs(comport));
            }

            DateTime dtStartTime = DateTime.Now;
            while (DevicesFound.Count == 0 && (DateTime.Now - dtStartTime).TotalMilliseconds < 60000)
            {
                Thread.Sleep(100);
            }

            foreach (BackgroundWorker b in threads)
            {
                b.CancelAsync();
                b.Dispose();
            }

            threads.Clear();

            if (DevicesFound.Count > 0)
                return DevicesFound[0];
            else
                return null;
        }

        private static void StubFindDevices(object sender, DoWorkEventArgs e)
        {
            DeviceInfo dinfo = null;
            
            string comport = (string)e.Argument;

            var MeterTypes = Assembly.GetExecutingAssembly().GetTypes()
                                 .Where(t => t.IsClass) // Only include classes
                                 .Where(t => t.IsSubclassOf(typeof(AbstractMeter)))
                                 .OrderBy(t => t.Namespace)
                                 .ThenBy(t => t.Name);
            
            //for each com port test each meter type to see if its on that port
            foreach (var reflect in MeterTypes)
            {
                Type t = reflect.UnderlyingSystemType;
                IMeter meter = (GlucaTrack.Communication.IMeter)Activator.CreateInstance(t);

                //if failed to connect skip this meter
                if (!meter.Connect(comport))
                    continue;

                try
                {
                    Console.WriteLine("Testing " + comport + " for " + reflect.UnderlyingSystemType.ToString());
                    if (meter.Port.IsOpen && meter.IsMeterConnected(comport))
                    {
                        dinfo = new DeviceInfo();
                        dinfo.DeviceType = t;
                        dinfo.ComPortName = comport;

                        lock (DevicesFound)
                        {
                            DevicesFound.Add(dinfo);
                            return; //found a device so return.  Can be removed to find all devices on a system.
                        }
                    }
                }//try
                finally
                {
                    meter.Dispose();
                }
            }
        }
    }

    public class DeviceInfo
    {
        /// <summary>
        /// Device type detected as the first available device.
        /// </summary>
        public Type DeviceType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the com port name of the first device detected.
        /// </summary>
        public string ComPortName
        {
            get;
            set;
        }

        public string DeviceDescription
        {
            get
            {
                if (DeviceType != null)
                    using (IMeter meter = (IMeter)Activator.CreateInstance(DeviceType))
                        return meter.MeterDescription;
                else
                    return string.Empty;
            }
        }

        public DeviceInfo()
        {
        }

    }

}


#region Graveyard
/*
        public static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            using (ManagementObjectSearcher COMsearcher = new ManagementObjectSearcher("root\\WMI", "SELECT InstanceName, PortName FROM MSSerial_PortName"))
            {
                ManagementObjectCollection collection;
                using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                    collection = searcher.Get();

                foreach (var device in collection)
                {
                    string pid = string.Empty;
                    string vid = string.Empty;
                    string comPort = String.Empty;

                    //look through each property to get the pid and vid
                    foreach (string s in ((string)device.GetPropertyValue("DeviceID")).Split(new char[] { '\\' }))
                        if (s.ToLower().Contains("vid") && s.ToLower().Contains("pid"))
                        {
                            vid = s.Split(new char[] { '&' })[0];
                            pid = s.Split(new char[] { '&' })[1];
                        }

                    USBDeviceInfo usb = new USBDeviceInfo(
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("PNPDeviceID"),
                    (string)device.GetPropertyValue("Description"),
                    (string)device.GetPropertyValue("SystemName"),
                    (string)device.GetPropertyValue("Name"),
                    (string)device.GetPropertyValue("Status"),
                    comPort
                    );

                    try
                    {
                        //look through each property for the COM port name
                        foreach (ManagementObject port in COMsearcher.Get())
                        {
                            if (port["InstanceName"].ToString().ToLowerInvariant().Contains(vid.ToLowerInvariant()) && port["InstanceName"].ToString().ToLowerInvariant().Contains(pid.ToLowerInvariant()))
                                usb.COMPort = port["PortName"].ToString();
                        }

                        //add device to the list of devices found if not one of the more popular usb device names
                        if (!(usb.Description.Contains("USB") && usb.Description.Contains("Hub")))
                            if (!(usb.Description.Contains("Mass Storage")))
                                if (!(usb.Description.Contains("Keyboard")))
                                    if (!(usb.Description.Contains("Mouse")))
                                        devices.Add(usb);
                    }//try
                    catch(ManagementException mex) { }
                }

                collection.Dispose();
            }
            return devices;
        }
 * 
 * public class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description, string SystemName, string name, string Status, string comPort)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.SystemName = SystemName;
            this.Name = name;
            this.Status = Status;
            this.COMPort = comPort;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string SystemName { get; private set; }
        public string Name { get; private set; }
        public string Status { get; private set; }
        public string COMPort { get; set; }
    }
        */
#endregion
