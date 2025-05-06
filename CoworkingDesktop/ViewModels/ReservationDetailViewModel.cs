using CoworkingDesktop.Models;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class ReservationDetailViewModel : ViewModelBase, IClosableDialog
    {
        private Reservation _reservation;

        public Reservation Reservation { get => _reservation; private set => Set(ref _reservation, value); }

        public event EventHandler<bool>? RequestClose;

        private string _title = "Reservation Detail";
        public string Title { get => _title; private set => Set(ref _title, value); }

        public ICommand CloseCommand { get; }

        public ReservationDetailViewModel(Reservation reservation)
        {
            Reservation = reservation;
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, true));
        }
    }
}
