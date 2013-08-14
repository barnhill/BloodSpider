using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Services.Windows
{
    public partial class Update
    {
        Timer timerStartUpdate = new Timer();
        string _updatePath = string.Empty;

        public Update()
        {
            timerStartUpdate.Tick += timerStartUpdate_Tick;
        }

        public Update(string appid, string strCurrentVersion)
        {
            using (WebService.GTServiceClient client = new WebService.GTServiceClient())
            {
                _updatePath = client.IsUpdatePresent(appid, strCurrentVersion);
            }
            
            if (!string.IsNullOrWhiteSpace(_updatePath))
                timerStartUpdate.Start();
        }

        private void DoUpdate()
        {
            //get the update file
            try
            {
                using (WebClient wc = new WebClient())
                {
                    //do not use the cache 
                    wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    wc.Headers.Add("Cache-Control", "no-cache");

                    //start asynch download of update
                    System.Threading.Thread.Sleep(500);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                    wc.DownloadFileAsync(new Uri(_updatePath), Environment.SpecialFolder.ApplicationData + "\\GlucaTrack_Update.exe");
                }//using
            }//try
            catch (Exception)
            {
            }//catch
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //display the update download progress
            System.Threading.Thread.Sleep(25);
        }

        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                //start the installer for the update
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo(Environment.SpecialFolder.ApplicationData + "\\GlucaTrack_Update.exe");
                p.Start();

                //close the current program
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }//try
            catch (Exception)
            {
            }//catch
        }

        private void timerStartUpdate_Tick(object sender, EventArgs e)
        {
            _updatePath = string.Empty;
            timerStartUpdate.Stop();
            timerStartUpdate.Enabled = false;
            DoUpdate();
        }
    }
}
