using System.Security.Claims;
using CoworkingApp.Models.DTOModels.Auth;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

[ApiController]
[Route("/api/[controller]")]
public class AuthApiController(
    IAuthService authService,
    IConfiguration configuration
    ) : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] UserRegisterRequestDto request)
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
    public async Task<ActionResult<TokenResponseDto>> LoginAsync(UserLoginRequestDto request)
    {
        try
        {
            var tokenResponseDto = await authService.LoginAsync(request);

            StoreCookies(tokenResponseDto);
            
            return Ok(new { message = "Login successful"});
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
    public async Task<ActionResult<TokenResponseDto>> RefreshTokensAsync()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
                return Unauthorized(new { message = "Can't refresh token (No refresh token found or is expired)" });

            // TODO: Change the user ID to be a uuid string
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var tokenResponseDto = await authService.RefreshTokensAsync(
                new RefreshTokenRequestDto()
                {
                    UserId = userId, 
                    RefreshToken = refreshToken
                });
           
            StoreCookies(tokenResponseDto);
            
            // return Ok(tokenResponseDto);
            return Ok(new { message = "Refresh successful" });
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
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:ExpiryMinutes")) 
            });
        
       Response.Cookies.Append("refreshToken", tokenResponseDto.RefreshToken, 
           new CookieOptions()
           {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<double>("JwtSettings:RefreshTokenExpiryMinutes")) 
           }); 
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
