using System.Security.Claims;
using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Models.Misc;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

[PublicApiController]
[Route("/api/auth")]
public class AuthApiController
    (
        IAuthService authService,
        IUserService userService,
        IMapper mapper
    ) 
    : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] UserRegisterRequestDto request)
    {
        try
        {
            var userDto = await authService.RegisterUser(request);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody]UserLoginRequestDto request)
    {
        try
        {
            var tokenResponseDto = await authService.LoginUser(request);

            await authService.StoreCookies(Response, tokenResponseDto); // For web users, storing in cookies.

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
        catch (WrongPasswordException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var userId = User.GetUserId(); 
        if (userId == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        var user = await userService.GetUserById(userId.Value);
        var userDto = mapper.Map<UserDto>(user);
        return Ok(userDto);
    } 

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutUser(Response);
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
            var userId = await authService.ExtractUserIdFromToken(refreshToken);
            if (userId == null)
                return Unauthorized(new { message = "Invalid refresh token" });
            
            // refresh and get the new cookies
            var tokenResponseDto = await authService.RefreshTokens(userId, refreshToken);
    
            // store the new cookies
            await authService.StoreCookies(Response, tokenResponseDto); // For web users, storing token in cookies.
            
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

    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetProtectedEndpoint()
    {
        var email = User.GetEmail();
        return Ok($"Hi, {email}, you are authenticated!");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult GetAdminOnlyEndpoint()
    {
        var email = User.GetEmail();
        return Ok($"Hi, {email}, you are admin!");
    }
}
