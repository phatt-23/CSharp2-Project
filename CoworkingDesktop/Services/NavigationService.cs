using CoworkingDesktop.ViewModels;
using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public class NavigationService : INavigationService
    {
        public event Action<ViewModelBase>? CurrentViewModelChanged;

        public void Navigate(ViewModelBase destinationViewModel)
        {
            CurrentViewModelChanged?.Invoke(destinationViewModel);
        }
    }
}
