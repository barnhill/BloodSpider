using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GlucoseMeterReader
{
    public partial class WaitMessage : Form
    {
        Timer tCloser = new Timer();

        public WaitMessage()
        {
            InitializeComponent();
        }

        public int ProgressValue
        {
            set 
            { 
                this.pbProgress.Value = value;
                this.pbProgress.Refresh();
            }
        }
        public int ProgressMax
        {
            set 
            { 
                this.pbProgress.Maximum = value;
                this.pbProgress.Invalidate();
            }
        }

        public void StartCloseTimer()
        {
            tCloser.Stop();
            tCloser.Interval = 1000;
            tCloser.Tick += new EventHandler(tCloser_Tick);
            tCloser.Start();
        }

        void tCloser_Tick(object sender, EventArgs e)
        {
            this.Hide();
            tCloser.Stop();
        }

        private void linkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
