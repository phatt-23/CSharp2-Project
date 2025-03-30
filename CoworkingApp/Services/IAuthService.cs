using CoworkingApp.Models.DTOModels.Auth;
using CoworkingApp.Models.DTOModels.User;

namespace CoworkingApp.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(UserRegisterRequestDto request);
    Task<TokenResponseDto> LoginAsync(UserLoginRequestDto request);
    Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request);
}