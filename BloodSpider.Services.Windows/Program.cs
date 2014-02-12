using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace BloodSpider.Services.Windows
{
    static partial class Program
    {
        private static string Service_Name = "BloodSpider Detector";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string [] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    // Run your service normally.
                    ServiceBase[] ServicesToRun = new ServiceBase[] { new Detector() };
                    ServiceBase.Run(ServicesToRun);
                }
                else if (args.Length == 1)
                {
                    switch (args[0])
                    {
                        case "-install":
                            InstallService();
                            StartService();
                            break;
                        case "-uninstall":
                            StopService();
                            UninstallService();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            catch (Exception ex)
            {
                Detector.ReportException("W0001", ex);
            }
        }

        private static bool IsInstalled()
        {
            using (ServiceController controller =
                new ServiceController(Service_Name))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private static bool IsRunning()
        {
            using (ServiceController controller =
                new ServiceController(Service_Name))
            {
                if (!IsInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(
                typeof(Detector).Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        private static void InstallService()
        {
            if (IsInstalled()) 
                return;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller = new ServiceController(Service_Name))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private static void StopService()
        {
            if (!IsInstalled()) return;
            using (ServiceController controller =
                new ServiceController(Service_Name))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
