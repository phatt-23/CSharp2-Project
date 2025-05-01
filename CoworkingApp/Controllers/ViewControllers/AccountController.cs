using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoworkingApp.Controllers.ViewControllers;

public class AccountController
    (
        IAuthService authService
    )
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View(new UserLoginRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var tokens = await authService.LoginUser(request);
            await authService.StoreCookies(Response, tokens);
            return RedirectToAction("Index", "Home");
        }
        catch (FormValidationException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View(new UserRegisterRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterRequestDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return View(requestDto);
        }

        try
        {
            await authService.RegisterUser(requestDto);
            var tokens = await authService.LoginUser(new UserLoginRequestDto
            {
                Email = requestDto.Email,
                Password = requestDto.Password
            });
            await authService.StoreCookies(Response, tokens);

            return RedirectToAction("Index", "Home");
        }
        catch (FormValidationException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
        }
        
        return View(requestDto);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutUser(this.Response);
        return RedirectToAction("Index", "Home"); 
    }
}