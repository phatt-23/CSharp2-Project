using AutoMapper.Configuration.Conventions;
using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class CoworkingCenterFormViewModel : ViewModelBase, IClosableDialog
    {
        public CoworkingCenter Center { get => _center; set => Set(ref _center, value); }
        public double? Latitude { get => _latitude; set => Set(ref _latitude, value); }
        public double? Longitude { get => _longitude; set => Set(ref _longitude, value); }

        public ICommand CloseCommand { get; }
        public ICommand SaveCommand { get; }

        public string? ErrorMessage { get => _errorMessage; set => Set(ref _errorMessage, value); }

        private FormMode _mode;
        public FormMode Mode { get => _mode; private set => Set(ref _mode, value); }

        public bool IsInAddMode => Mode == FormMode.Add;
        public bool IsInEditMode => Mode == FormMode.Edit;

        public ICommand FetchAddressCommand { get; }
        public Address FetchedAddress { get => _address; private set => Set(ref _address, value); }

        public CoworkingCenterFormViewModel(CoworkingCenter center, FormMode mode, IAddressService addressService, Address? address = null)
        {
            Center = center;
            _addressService = addressService;

            if (address == null)
            {
                Latitude = 0;
                Longitude = 0;
            }
            else
            {
                Latitude = (double)address.Latitude;
                Longitude = (double)address.Longitude;
            }

            Mode = mode;

            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));
            SaveCommand = new RelayCommand(() => RequestClose?.Invoke(this, true), CanSave);
            FetchAddressCommand = new RelayCommand(async () => await FetchAddress());
        }

        public async Task FetchAddress()
        {
            if (Latitude == null || Longitude == null) return;

            var address = await _addressService.GetAddressByCoords(Latitude.Value, Longitude.Value);
            if (address == null)
            {
                ErrorMessage = "Coordinates didn't provide valid address.";
                return;
            }

            FetchedAddress = address;
        }

        public bool CanSave()
        {
            ErrorMessage = null;

            if (string.IsNullOrEmpty(Center.Name))
            {
                ErrorMessage = "Name is required.";
                return false;
            }
            if (Latitude == null || Longitude == null)
            {
                ErrorMessage = "Latitude and Longitude are required.";
                return false;
            }
            if (!(-90.0 <= Latitude && Latitude <= 90.0))
            {
                ErrorMessage = "Latitude must be in span from -90 to 90.";
                return false;
            }
            if (!(-180.0 <= Longitude && Longitude <= 180.0))
            {
                ErrorMessage = "Longitude must be in span from -180 to 180.";
                return false;
            }

            return true;
        }

        private string? _errorMessage;
        private double? _latitude, _longitude;
        private CoworkingCenter _center;
        private Address _address;
        private readonly IAddressService _addressService;

        public event EventHandler<bool>? RequestClose;
    }
}
