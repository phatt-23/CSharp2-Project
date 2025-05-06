using CoworkingDesktop.Helpers;
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
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class CoworkingCentersViewModel : PaginatedCrudViewModel<CoworkingCenter>
    {
        public CoworkingCentersViewModel(
            ICoworkingCenterService centerService,
            IDialogService dialog,
            IAddressService addressService
            )
        {
            _centersService = centerService;
            _dialog = dialog;
            _addressService = addressService;
        }

        public override async Task Add()
        {
            var center = new CoworkingCenter();
            var vm = new CoworkingCenterFormViewModel(center, FormMode.Add, _addressService);

            vm.RequestClose += async (s, ok) =>
            {
                if (!ok || vm.Latitude == null || vm.Longitude == null) return;

                var dto = new CoworkingCenterCreateDto
                {
                    Name = vm.Center.Name,
                    Description = vm.Center.Description,
                    Latitude = (decimal)vm.Latitude.Value,
                    Longitude = (decimal)vm.Longitude.Value,
                };

                var created = await _centersService.CreateCoworkingCenter(dto);
                if (created == null)
                {
                    MessageBox.Show("Adding coworking center failed.", "Add Coworking Center Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Items.Add(created);
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task Delete()
        {
            if (SelectedItem == null) return;

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete this '{SelectedItem.Name}' coworking center?", "Delete Coworking Center", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            var center = await _centersService.DeleteCoworkingCenter(SelectedItem.CoworkingCenterId);
            if (center == null) return;

            Items.Remove(SelectedItem);
            SelectedItem = null;
        }

        public override async Task Edit()
        {
            if (SelectedItem == null) return;

            var address = await _addressService.GetAddressById(SelectedItem.AddressId);
            if (address == null)
            {
                MessageBox.Show("Edit window failed to open.", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var clone = SelectedItem.DeepCopy()!;
            var vm = new CoworkingCenterFormViewModel(clone, FormMode.Edit, _addressService, address);

            vm.RequestClose += async (s, ok) =>
            {
                if (!ok || vm.Latitude == null || vm.Longitude == null) return;

                var dto = new CoworkingCenterUpdateDto
                {
                    CoworkingCenterId = vm.Center.CoworkingCenterId,
                    Name = vm.Center.Name,
                    Description = vm.Center.Description,
                    Latitude = (decimal)vm.Latitude.Value,
                    Longitude = (decimal)vm.Longitude.Value,
                };

                var updated = await _centersService.UpdateCoworkingCenter(dto);
                if (updated == null)
                {
                    MessageBox.Show("Editing coworking center failed.", "Edit Coworking Center Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var index = Items.IndexOf(SelectedItem);
                if (index >= 0)
                {
                    Items[index] = updated;
                    SelectedItem = updated;
                }
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task ViewDetail()
        {
            MessageBox.Show("ViewDetail not implemented", "View Detail", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override async Task<PagedResult<CoworkingCenter>> LoadPageAsync(int page, int pageSize)
        {
            return await _centersService.GetCoworkingCenters(page, pageSize);
        }

        private readonly ICoworkingCenterService _centersService;
        private readonly IDialogService _dialog;
        private readonly IAddressService _addressService;

    }
}
