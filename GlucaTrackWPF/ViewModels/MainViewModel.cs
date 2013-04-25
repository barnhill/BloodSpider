using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using GlucaTrackWPF.Models;
using GlucaTrack.Communication;
using GlucaTrack.Communication.Meters;

namespace GlucaTrackWPF.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private MainModel _main = new MainModel();
        
        BackgroundWorker bgw = new BackgroundWorker();

        public MainModel Main
        {
            get
            {
                return _main;
            }
            set
            {
                if (_main != value)
                {
                    _main = value;
                    NotifyPropertyChanged("Main");
                }
            }
        }

        public MainViewModel(IViewModelBase vmbase)
        {
            if (vmbase is LoginViewModel)
                Main.Login = (vmbase as LoginViewModel).Login;

            FindDevice();
        }

        private void FindDevice()
        {
            bgw.DoWork += bgw_DoWork;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            bgw.WorkerSupportsCancellation = true;

            bgw.RunWorkerAsync();
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
                FindDevice();
            else
            {
                Main.Device = (DeviceInfo)e.Result;
                Main.IsMeterFound = true;
                return;
            }
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            DeviceInfo fdi = Statics.DetectFirstDevice();

            e.Result = fdi;
        }
    }
}
