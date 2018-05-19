using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using BloodSpider.Services.Common;

namespace BloodSpider.Services.Windows
{
    public partial class Update
    {
        string _updatePath = string.Empty;
        
        public Update(string updatePath)
        {
            _updatePath = updatePath;

            DoUpdate();
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
                    wc.DownloadFileAsync(new Uri(_updatePath), Statics.LocalUpdateFilePath);
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
            p.StartInfo = new ProcessStartInfo(Statics.LocalUpdateFilePath);
            p.StartInfo.Verb = "runas";
            p.StartInfo.Arguments = "/S"; //NSIS silent install
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            //close the current program
            Process.GetCurrentProcess().Kill();
        }
    }
}
