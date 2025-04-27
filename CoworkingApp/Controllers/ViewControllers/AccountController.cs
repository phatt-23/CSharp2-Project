using System.Security.Claims;
using CoworkingApp.Controllers.APIEndpoints.Public;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Auth;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.MVC;


public class AccountController
    (
    IAuthService authService,
    IUserService userService
    ) 
    : Controller
{
    // Route: GET /login
    public IActionResult Login()
    {
        return View(new UserLoginRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginRequestDto request)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var tokens = await authService.LoginAsync(request);
            await authService.StoreCookiesAsync(Response, tokens);
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

    // Route: GET /register
    public IActionResult Register()
    {
        return View(new UserRegisterRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterRequestDto requestDto)
    {
        // check for email uniqueness
        var users = await userService.GetUsersAsync(new UserQueryRequestDto() { Email = requestDto.Email });
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
            await authService.RegisterAsync(requestDto);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
        }
        
        return View(requestDto);
    }
    
    // Route: POST /logout
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(Response);
        return RedirectToAction("Index", "Home"); 
    }
}