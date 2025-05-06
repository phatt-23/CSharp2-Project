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

namespace CoworkingDesktop.ViewModels
{
    public class UsersViewModel : PaginatedCrudViewModel<User>
    {
        public UsersViewModel(
            IDialogService dialog,
            IUsersService usersService,
            IReservationsService reservationService
            )
        {
            _usersService = usersService;
            _dialog = dialog;
            _reservationService = reservationService;
        }

        public override async Task Add()
        {
            var user = new User();
            var vm = new UserFormViewModel(user, FormMode.Add);

            vm.RequestClose += async (s, ok) =>
            {
                if (!ok) return;

                if (vm.SelectedRole == null) return;

                var dto = new UserCreateDto
                {
                    Email = vm.User.Email,
                    Password = vm.Password,
                    Role = vm.SelectedRole.Value,
                };

                var created = await _usersService.CreateUser(dto);
                if (created == null)
                {
                    MessageBox.Show("Adding user failed.", "Add User Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Items.Add(created);
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task Delete()
        {
            if (SelectedItem == null) return;

            MessageBoxResult result = MessageBox.Show($"Do you really wish to delete user with '{SelectedItem.Email}' email?", "Delete User", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            var user = await _usersService.DeleteUser(SelectedItem.UserId);
            if (user == null) return;

            Items.Remove(SelectedItem);
            SelectedItem = null;
        }

        public override async Task Edit()
        {
            if (SelectedItem == null) return;

            var clone = SelectedItem.DeepCopy()!;
            var vm = new UserFormViewModel(clone, FormMode.Edit);

            vm.RequestClose += async (s, ok) =>
            {
                if (!ok) return;

                if (vm.SelectedRole == null) return;
                if (SelectedItem.Role == vm.SelectedRole) return;

                var dto = new UserRoleChangeDto
                {
                    UserId = clone.UserId,
                    Role = vm.SelectedRole.Value,
                };

                var updated = await _usersService.ChangeRole(dto);
                if (updated == null)
                {
                    MessageBox.Show("Editing user failed.", "Edit User Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // show user and their reservations
            //MessageBox.Show("Viewing user details is not supported.", "View User", MessageBoxButton.OK, MessageBoxImage.Information);
            if (SelectedItem == null) return;
            var vm = new UserDetailViewModel(SelectedItem, _usersService);
            _dialog.ShowDialog(vm);
        }

        protected override async Task<PagedResult<User>> LoadPageAsync(int page, int pageSize)
        {
            return await _usersService.GetUsers(page, pageSize);
        }

        private IUsersService _usersService;
        private readonly IDialogService _dialog;
        private readonly IReservationsService _reservationService;
    }
}
