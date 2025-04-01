using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoworkingApp.Models.DTOModels.Auth;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

[ApiController]
[Route("/api/auth")]
public class AuthApiController(
    IAuthService authService,
    IConfiguration configuration
    ) : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody]UserRegisterRequestDto request)
    {
        try
        {
            var userDto = await authService.RegisterAsync(request);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> LoginAsync([FromBody]UserLoginRequestDto request)
    {
        try
        {
            var tokenResponseDto = await authService.LoginAsync(request);

            StoreCookies(tokenResponseDto); // For web users, storing in cookies.
            
            return Ok(tokenResponseDto); // For other users, handle manually.
            // return Ok(new { message = "Login successful"});
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (AuthenticationFailureException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { userId, email });
    } 

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("refreshToken");
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh-tokens")]
    public async Task<ActionResult<TokenResponseDto>> RefreshTokensAsync([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? request.RefreshToken;

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No refresh token found or is expired" });

            var userId = ExtractUserIdFromToken(refreshToken);
            
            if (userId == null)
                return Unauthorized(new { message = "Invalid refresh token" });
            
            var tokenResponseDto = await authService.RefreshTokensAsync(userId, refreshToken);
    
            StoreCookies(tokenResponseDto); // For web users, storing token in cookies.
            
            return Ok(tokenResponseDto); // For other users, they handle it themselves.
            // return Ok(new { message = "Refresh successful" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);   
        }
        catch (AuthenticationFailureException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    private void StoreCookies(TokenResponseDto tokenResponseDto)
    {
        Response.Cookies.Append("jwt", tokenResponseDto.AccessToken, 
            new CookieOptions() 
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:ExpiryMinutes")!) 
            });
        
       Response.Cookies.Append("refreshToken", tokenResponseDto.RefreshToken, 
           new CookieOptions()
           {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes")!) 
           }); 
    }

    private string? ExtractUserIdFromToken(string token)
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

            return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        catch
        {
            return null;
        }
    }
    
    ///////////////////////////////////////////////////////////
    /////// TEST ENDPOINTS ////////////////////////////////////
    ///////////////////////////////////////////////////////////

    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtectedEndpoint()
    {
        var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        return Ok($"Hi, {email}, you are authenticated!");
    }


    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdminOnlyEndpoint()
    {
        var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        return Ok($"Hi, {email}, you are admin!");
    }
}
