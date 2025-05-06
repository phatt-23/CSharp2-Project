using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels;
using CoworkingDesktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoworkingDesktop.Helpers
{
    /// <summary>
    /// Transparent header injection for all API calls.
    /// Handles authorization and refreshing tokens.
    /// </summary>
    public class AuthHeadersHandler : DelegatingHandler
    {
        private readonly IAuthService _auth;
        private readonly IServiceProvider _services;
        private readonly IDialogService _dialogService;

        public AuthHeadersHandler(
            IDialogService dialogService,
            IAuthService auth, 
            IServiceProvider services
            )
        {
            _auth = auth;
            _services = services;
            _dialogService = dialogService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_auth.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AccessToken);
            }

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refreshed = await _auth.TryRefreshTokens();
                    if (refreshed)
                    {
                        // add the new token to the request
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AccessToken);

                        // get rid of the old response
                        response.Dispose();

                        // send the request again
                        response = await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        // Login again to refresh access token and refresh token

                        var loginVm = new LoginViewModel(_auth);
                        var loginWindow = new LoginWindow(loginVm, _services);
                        loginWindow.Show();
                    }
                }

                return response;
            }
            catch (HttpRequestException)
            {
                // prompt the user to login again to get new tokens
                MessageBox.Show("Session expired. Please login again.", "Session Expired", MessageBoxButton.OK, MessageBoxImage.Warning);
                App.Current.Shutdown(0);
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}
