using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface INavigationService
    {
        event Action<ViewModelBase>? CurrentViewModelChanged;
        void Navigate(ViewModelBase destinationViewModel);
    }
}
