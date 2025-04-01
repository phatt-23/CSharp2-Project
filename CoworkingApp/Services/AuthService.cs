using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Auth;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Models.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoworkingApp.Services;


public interface IAuthService
{
    Task<UserDto> RegisterAsync(UserRegisterRequestDto request);
    Task<TokenResponseDto> LoginAsync(UserLoginRequestDto request);
    Task<TokenResponseDto> RefreshTokensAsync(string userId, string refreshToken);
}

public class AuthService(
    CoworkingDbContext context,
    IMapper mapper,
    IConfiguration configuration
    ) : IAuthService
{
    public async Task<UserDto> RegisterAsync(UserRegisterRequestDto request)
    {
        if (context.Users.Any(u => u.Email == request.Email))
            throw new Exception("Email already exists");

        var user = mapper.Map<User>(request);
        
        var userRole = await context.UserRoles.SingleAsync(ur => ur.Name == UserRoleType.User.ToString());
        user.RoleId = userRole.Id;
        
        var passwordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var userDto = mapper.Map<UserDto>(user);
        
        return userDto;
    }

    public async Task<TokenResponseDto> LoginAsync(UserLoginRequestDto request)
    {
        var user = await context.Users
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null)
            throw new NotFoundException("User with this email does not exist");
       
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            throw new AuthenticationFailureException("Password verification failed");

        return await CreateTokenResponseAsync(user);
    }

    public async Task<TokenResponseDto> RefreshTokensAsync(string userId, string refreshToken)
    {
        var user = await ValidateRefreshToken(userId, refreshToken);
        return await CreateTokenResponseAsync(user);
    }

    private async Task<TokenResponseDto> CreateTokenResponseAsync(User user)
    {
        return new TokenResponseDto()
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
        };
        
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey")!));
        
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: configuration.GetValue<string>("JwtSettings:Audience"),
            expires: DateTime.Now.AddMinutes(configuration.GetValue<double>("JwtSettings:ExpiryMinutes")),
            claims: claims,
            signingCredentials: signingCredentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = GenerateRefreshToken(user.Id.ToString());
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes"));
        await context.SaveChangesAsync();
        return refreshToken;
    }

    private async Task<User> ValidateRefreshToken(string userId, string refreshToken)
    {
        // TODO: Remove this, use UUID instead of int
        var dbUserId = int.Parse(userId);
        
        var user = await context.Users
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.Id == dbUserId);
        
        if (user == null)
            throw new NotFoundException($"User with id '{userId}' not exist");

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
            issuer: configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: configuration.GetValue<string>("JwtSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes")!), // Refresh token expiry
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);  
    }

}