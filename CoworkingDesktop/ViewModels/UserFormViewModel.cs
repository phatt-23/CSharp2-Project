using CoworkingDesktop.Helpers;
using CoworkingDesktop.Models;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class UserFormViewModel : ClosableFormViewModel
    {
        public string Title => Mode == FormMode.Add ? "Add User" : "Edit User";

        public User User { get => _user; set => Set(ref _user, value); }
        public string Password { get => _password; set => Set(ref _password, value); }
        public ObservableCollection<UserRoleType> Roles { get; }
        public UserRoleType? SelectedRole { get => _selectedRole; set => Set(ref _selectedRole, value); }

        public UserFormViewModel(User user, FormMode mode) : base(mode)
        {
            User = user;
            Roles = [.. Enum.GetNames(typeof(UserRoleType)).Select(n => Enum.Parse<UserRoleType>(n)) ];

            if (IsInEditMode)
            {
                SelectedRole = user.Role;
            }
        }

        public override bool CanSave()
        {
            ErrorMessage = null;

            if (string.IsNullOrEmpty(User.Email))
            {
                ErrorMessage = "Email is required.";
                return false;
            }
            if (!User.Email.IsValidEmailAddress())
            {
                ErrorMessage = "Email is not valid.";
                return false;
            }
            if (IsInAddMode)
            {
                if (string.IsNullOrEmpty(Password))
                {
                    ErrorMessage = "Password is required.";
                    return false;
                }
                if (Password.Length < 6)
                {
                    ErrorMessage = "Password must be at least 6 characters long.";
                    return false;
                }
            }
            if (SelectedRole == null)
            {
                ErrorMessage = "Please select a role.";
                return false;
            }
            return true;
        }

        private UserRoleType? _selectedRole;
        private string _password = string.Empty;
        private User _user;
    }
}
