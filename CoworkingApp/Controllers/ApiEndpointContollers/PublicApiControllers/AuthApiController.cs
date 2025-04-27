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
    IAuthService authService
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

            await authService.StoreCookiesAsync(Response, tokenResponseDto); // For web users, storing in cookies.
            
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
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(Response);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh-tokens")]
    public async Task<ActionResult<TokenResponseDto>> RefreshTokensAsync([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            // get the refresh token out of the cookie
            var refreshToken = Request.Cookies["refreshToken"] ?? request.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No refresh token found or is expired" });

            // get the users id
            var userId = await authService.ExtractUserIdFromTokenAsync(refreshToken);
            if (userId == null)
                return Unauthorized(new { message = "Invalid refresh token" });
            
            // refresh and get the new cookies
            var tokenResponseDto = await authService.RefreshTokensAsync(userId, refreshToken);
    
            // store the new cookies
            await authService.StoreCookiesAsync(Response, tokenResponseDto); // For web users, storing token in cookies.
            
            return Ok(tokenResponseDto);   // For non-browser users, they handle it themselves.
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
