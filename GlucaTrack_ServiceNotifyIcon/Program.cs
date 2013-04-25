using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GlucaTrack.Services.Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formSettings form = new formSettings();
            Application.Run();
        }
    }
}
