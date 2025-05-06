using CoworkingDesktop.Helpers;
using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace CoworkingDesktop.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }

        public AuthService()
        {
            // Doesn't use API http client, thus must handle errors by itself.
            _http = new HttpClient
            {
                BaseAddress = new Uri(App.RESOURCE_URL)
            };

            _http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/login", new { email, password });
                if (!await HttpResponseHelper.InfoOnBadStatusCode(response))
                    return false;

                var tokens = await response.Content.ReadFromJsonAsync<TokensDto>();

                AccessToken = tokens?.AccessToken;
                RefreshToken = tokens?.RefreshToken;
                return true;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Connection to server '{_http.BaseAddress}' failed: {ex.Message}", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> TryRefreshTokens()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RefreshToken))
                    return false;

                var response = await _http.PostAsJsonAsync("api/auth/refresh-tokens", new { RefreshToken });
                if (!response.IsSuccessStatusCode)
                    return false;

                var tokens = await response.Content.ReadFromJsonAsync<TokensDto>();

                AccessToken = tokens?.AccessToken;
                RefreshToken = tokens?.RefreshToken;
                return true;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Connection to server '{_http.BaseAddress}' failed: {ex.Message}", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<UserDto?> GetCurrentUser()
        {
            try
            {
                var response = await _http.GetAsync("api/auth/me");

                if (!await HttpResponseHelper.InfoOnBadStatusCode(response))
                    return null;

                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Connection to server '{_http.BaseAddress}' failed: {ex.Message}", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void Logout()
        {
            AccessToken = RefreshToken = null;
        }

        public async Task<bool> SaveTokensToFile(string path)
        {
            if (string.IsNullOrWhiteSpace(AccessToken) && string.IsNullOrWhiteSpace(RefreshToken))
                throw new InvalidOperationException("No tokens to save.");

            using var fs = File.OpenWrite(path);
            using var sw = new StreamWriter(fs);

            var tokens = JsonSerializer.Serialize(new TokensDto { AccessToken = AccessToken!, RefreshToken = RefreshToken! });

            await sw.WriteLineAsync(tokens);

            return true;
        }

        public async Task<bool> ReadTokensFromFile(string path)
        {
            if (!File.Exists(path))
                return false;

            string content = await File.ReadAllTextAsync(path);

            try
            {
                var tokens = JsonSerializer.Deserialize<TokensDto>(content);

                if (tokens == null) 
                    return false;

                AccessToken = tokens.AccessToken;
                RefreshToken = tokens.RefreshToken;

                if (!await TryRefreshTokens())
                    return false;

                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
