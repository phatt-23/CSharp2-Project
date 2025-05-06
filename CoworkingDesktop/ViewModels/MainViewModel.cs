using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
		private readonly INavigationService _nav;

        private ViewModelBase _currentViewModel;
		public ViewModelBase CurrentViewModel { get => _currentViewModel; set => Set(ref _currentViewModel, value); }

		public ICommand ShowReservationsCommand { get; private set; }
        public ICommand ShowWorkspacesCommand { get; private set; }
        public ICommand ShowCentersCommand { get; private set; }
        public ICommand ShowUsersCommand { get; private set; }
        public ICommand QuitCommand { get; private set; }
        public ICommand ShowStatsCommand { get; private set; }

        public MainViewModel(
			INavigationService navService,
			ReservationsViewModel reservationsVm,
			WorkspacesViewModel workspacesVm,
			CoworkingCentersViewModel centersVm,
            StatsViewModel statsVm,
			UsersViewModel usersVm)
		{
            _nav = navService;
            _nav.CurrentViewModelChanged += vm => CurrentViewModel = vm;

            _currentViewModel = reservationsVm;

            ShowReservationsCommand = new RelayCommand(() => _nav.Navigate(reservationsVm));
            ShowWorkspacesCommand = new RelayCommand(() => _nav.Navigate(workspacesVm));
            ShowCentersCommand = new RelayCommand(() => _nav.Navigate(centersVm));
            ShowUsersCommand = new RelayCommand(() => _nav.Navigate(usersVm));
            QuitCommand = new RelayCommand(() => App.Current.Shutdown(0));

            ShowStatsCommand = new RelayCommand(() => _nav.Navigate(statsVm));
        }


    }
}
