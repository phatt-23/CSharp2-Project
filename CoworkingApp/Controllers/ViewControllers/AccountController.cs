using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

[Route("account")]
public class AccountController
    (
        IAuthService authService,
        IUserService userService
    )
    : Controller
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View(new UserLoginRequestDto());
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
    {
        if (!ModelState.IsValid) 
            return View(request);

        try
        {
            var tokens = await authService.LoginUser(request);
            await authService.StoreCookies(Response, tokens);
            return RedirectToAction("Index", "Home");
        }
        catch (WrongPasswordException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
            return View(request);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(new UserRegisterRequestDto());
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterRequestDto requestDto)
    {
        // check for email uniqueness
        var users = await userService.GetUsers(new UserQueryRequestDto() { Email = requestDto.Email });

        if (users.Any())
        {
            ModelState.AddModelError("Email", "Email is already taken.");
        }

        if (!ModelState.IsValid)
        {
            return View(requestDto);
        }

        try
        {
            await authService.RegisterUser(requestDto);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
        }
        
        return View(requestDto);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutUser(Response);
        return RedirectToAction("Index", "Home"); 
    }
}