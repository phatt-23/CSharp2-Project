using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using System.Text.RegularExpressions;
using CoworkingDesktop.Helpers;
using System.IO;
using System.Security.RightsManagement;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Windows.Controls;

namespace CoworkingDesktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _auth;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => Set(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public EventHandler? LoginSucceeded;

        public LoginViewModel(IAuthService auth)
        {
            _auth = auth;
            LoginCommand = new RelayCommand(async () => await Login(), CanLogin);

            CheckForSavedTokens();
        }

        private async void CheckForSavedTokens()
        {
            if (await _auth.ReadTokensFromFile(App.CREDS_TOKENS_PATH))
            {
                await _auth.SaveTokensToFile(App.CREDS_TOKENS_PATH);
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool CanLogin()
        {
            return Email.IsValidEmailAddress();
        }

        private async Task Login()
        {
            var success = await _auth.Login(Email, Password);
            if (success)
            {
                await _auth.SaveTokensToFile(App.CREDS_TOKENS_PATH);
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }
    }
}
