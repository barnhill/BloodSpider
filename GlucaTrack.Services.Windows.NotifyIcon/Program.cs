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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                formSettings form = new formSettings();
                Application.Run();
            }
            else if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "-kill":
                        CloseAllandExit();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        static void CloseAllandExit()
        {
            Process[] localByName = Process.GetProcessesByName("GlucaTrack.Services.Windows.NotifyIcon");
            Process[] AllProcesses = Process.GetProcesses();
            foreach (Process p in localByName)
            {
                p.Kill();
            }

            Application.Exit();
        }
    }
}
