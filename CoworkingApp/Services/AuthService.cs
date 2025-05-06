using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Models.Misc;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoworkingApp.Services;
public interface IAuthService
{
    Task<User>              RegisterUser(UserRegisterRequestDto request);
    Task<TokenResponseDto>  LoginUser(UserLoginRequestDto request);
    Task                    LogoutUser(HttpResponse response);
    Task<TokenResponseDto>  RefreshTokens(string userId, string refreshToken);
    Task<bool>              TryRefreshToken(HttpContext context);
    Task                    StoreCookies(HttpResponse response, TokenResponseDto tokenResponseDto);
    Task<string?>           ExtractUserIdFromToken(string token);
}

public class AuthService
    (
        CoworkingDbContext context,
        IUserService userService,
        IConfiguration configuration
    ) 
    : IAuthService
{
    public async Task<User> RegisterUser(UserRegisterRequestDto request)
    {
        // already validated by FluentValidation
        if (request.Password != request.ConfirmPassword)
        {
            throw new PasswordMismatchException("Password and confirm password do not match");
        }

        var user = await userService.CreateUser(new AdminUserCreateDto()
        {
            Email = request.Email,
            Password = request.Password,
            Role = UserRoleType.Customer,
        });

        return user;
    }

    public async Task<TokenResponseDto> LoginUser(UserLoginRequestDto request)
    {
        var user = await userService.LoginUser(request);
        return await CreateTokenResponse(user);
    }

    public Task LogoutUser(HttpResponse response)
    {
        response.Cookies.Delete("jwt");
        response.Cookies.Delete("refreshToken");
        return Task.CompletedTask; 
    }
    public async Task<TokenResponseDto> RefreshTokens(string userId, string refreshToken)
    {
        var user = await ValidateRefreshToken(userId, refreshToken);
        return await CreateTokenResponse(user);
    }

    public async Task<bool> TryRefreshToken(HttpContext context)
    {
        var refreshToken = context.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) 
            return false;

        var userId = await ExtractUserIdFromToken(refreshToken);
        if (userId == null) 
            return false;

        var tokenResponse = await RefreshTokens(userId, refreshToken);  // let exceptions propagate
        await StoreCookies(context.Response, tokenResponse);

        return true;
    } 

    public Task StoreCookies(HttpResponse response, TokenResponseDto tokenResponseDto)
    {
        response.Cookies.Append("jwt", tokenResponseDto.AccessToken, 
            new CookieOptions() 
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:ExpiryMinutes")!) 
            });
        
       response.Cookies.Append("refreshToken", tokenResponseDto.RefreshToken, 
           new CookieOptions()
           {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes")!) 
           });
       
       return Task.CompletedTask;
    }

    public Task<string?> ExtractUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!);

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Don't validate expiry since it's a refresh token
            }, out _);

            return Task.FromResult(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }
    
    private async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        return new TokenResponseDto()
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.Role, user.Role.Name),
        };
        
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey")!));
        
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer:             configuration.GetValue<string>("JwtSettings:Issuer"),
            audience:           configuration.GetValue<string>("JwtSettings:Audience"),
            expires:            DateTime.Now.AddMinutes(configuration.GetValue<double>("JwtSettings:ExpiryMinutes")),
            claims:             claims,
            signingCredentials: signingCredentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken(user.UserId.ToString());
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes"));
        await context.SaveChangesAsync();
        return refreshToken;
    }

    private async Task<User> ValidateRefreshToken(string userId, string refreshToken)
    {
        // TODO: Remove this, use UUID instead of int
        var dbUserId = userId.TryParseToInt() ?? -1;

        var user = await userService.GetUserById(dbUserId);

        if (user.RefreshToken != refreshToken)
            throw new AuthenticationFailureException("Refresh token does not match");
        
        if (user.RefreshTokenExpiry < DateTime.Now)
            throw new AuthenticationFailureException("Refresh token expired");
            
        return user; 
    }

    private string GenerateRefreshToken(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId) // Store userId inside refresh token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             configuration.GetValue<string>("JwtSettings:Issuer"),
            audience:           configuration.GetValue<string>("JwtSettings:Audience"),
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes")!), // Refresh token expiry
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);  
    }
}
