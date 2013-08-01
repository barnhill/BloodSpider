using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace GlucaTrack.Services.Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                RunApplication();
            }
            else if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-kill":
                        CloseAllandExit();
                        break;
                    default:
                        RunApplication();
                        break;
                }
            }
        }

        static void RunApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formSettings form = new formSettings();
            Application.Run();
        }

        static void CloseAllandExit()
        {
            Process[] localByName = Process.GetProcessesByName("GlucaTrack.Services.Windows.NotifyIcon");
            Process[] AllProcesses = Process.GetProcesses();
            foreach (Process p in localByName)
            {
                try
                {
                    p.Kill();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Shutdown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Application.Exit();
        }
    }
}
