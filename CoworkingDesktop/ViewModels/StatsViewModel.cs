using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{

    public class StatsViewModel : ViewModelBase
    {
        public List<CoworkingCenterRevenueDto> Revenues { get => _revenues; private set => Set(ref _revenues, value); }
        public TimeBack? SelectedTimeBack { get => _selectedTimeBack; set => Set(ref _selectedTimeBack, value); }
        public ICommand LoadCommand { get; }
        public ObservableCollection<TimeBack> TimeBackOptions { get; private set; }

        public StatsViewModel(IStatsService statsService)
        {
            _statsService = statsService;
            LoadCommand = new RelayCommand(async () => await LoadRevenues(), CanLoad);
            TimeBackOptions = [.. Enum.GetNames(typeof(TimeBack)).Select(Enum.Parse<TimeBack>)];
        }

        public string? ErrorMessage { get => _errorMessage; private set => Set(ref _errorMessage, value); }

        public async Task LoadRevenues()
        {
            if (SelectedTimeBack == null) return;

            var result = await _statsService.GetCoworkingCenterRevenues(SelectedTimeBack.Value);
            if (result == null)
            {
                MessageBox.Show("Failed to load data.");
                return;
            }

            Revenues = result.Revenues.OrderByDescending(r => r.Revenue).ToList();
            SelectedTimeBack = result.TimeBack;
        }

        public bool CanLoad()
        {
            ErrorMessage = null;

            if (SelectedTimeBack == null)
            {
                ErrorMessage = "Please choose timeback.";
                return false;
            }
            return true;
        }

        private string? _errorMessage;
        private TimeBack? _selectedTimeBack;
        private List<CoworkingCenterRevenueDto> _revenues;
        private readonly IStatsService _statsService;
    }

}
