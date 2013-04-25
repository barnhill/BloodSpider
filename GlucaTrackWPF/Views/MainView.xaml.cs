using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GlucaTrackWPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public ViewModels.LoginViewModel ViewModel
        {
            get
            {
                return DataContext as ViewModels.LoginViewModel;
            }
        }

        public MainView(ViewModels.IViewModelBase vmbase)
        {
            InitializeComponent();
            DataContext = new ViewModels.MainViewModel(vmbase);
        }
    }
}
