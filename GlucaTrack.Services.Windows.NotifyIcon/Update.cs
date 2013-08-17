using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.ServiceProcess;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Services.Windows
{
    public partial class Update
    {
        string _updatePath = string.Empty;
        string _localUpdateFilename = Statics.BaseFilepath + "\\" + "GlucaTrack_Update.exe";

        public Update(string updatePath)
        {
            _updatePath = updatePath;

            //get rid of an old update
            if (File.Exists(_localUpdateFilename))
            {
                File.Delete(_localUpdateFilename);
            }

            if (!string.IsNullOrWhiteSpace(updatePath))
            {
                DoUpdate();
            }
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
                    wc.DownloadFileAsync(new Uri(_updatePath), _localUpdateFilename);
                }//using
            }//try
            catch (Exception)
            {
            }//catch
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //display the update download progress
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            StopService();
            StartUpdateProcess();
        }

        private void StopService()
        {
            //stop the service
            ServiceController service = new ServiceController(Statics.serviceName);
            try
            {
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(5000));
                }
            }
            catch { }
        }

        private void StartUpdateProcess()
        {
            //start the installer for the update
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(_localUpdateFilename);
            p.StartInfo.Verb = "runas";
            p.Start();

            //close the current program
            Process.GetCurrentProcess().Kill();
        }
    }
}
