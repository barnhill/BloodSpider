using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlucaTrackWPF.ViewModels
{
    public interface IViewModelBase
    {
        Guid SessionId
        {
            get;
            set;
        }
    }
}
