using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IAuthService
    {
        string? AccessToken { get; }
        string? RefreshToken { get; }

        Task<bool> Login(string username, string password);
        void Logout();
        Task<UserDto?> GetCurrentUser();
        Task<bool> TryRefreshTokens();
        Task<bool> SaveTokensToFile(string path);
        Task<bool> ReadTokensFromFile(string path);
    }
}
