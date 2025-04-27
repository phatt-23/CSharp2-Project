using System.Diagnostics;
using CoworkingApp.Models;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.MVC;

public interface IHomeController
{
    public Task<IActionResult> Index(); // Landing page for unauthenticated users
    public Task<IActionResult> Dashboard(); // View after login (for authenticated users) 
}


public class HomeController
    (
    IWorkspaceRepository workspaceRepository,
    ICoworkingCenterRepository coworkingCenterRepository 
    ) 
    : Controller, IHomeController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new ()
        {
            HasPricing = true,
            IncludePricings = true,
            IncludeStatus = true,
        });

        var coworkingCenters = await coworkingCenterRepository.GetCoworkingCentersAsync(new CoworkingCenterFilter()
        {

        });

        var model = new HomeIndexViewModel()
        {
            Workspaces = workspaces,
            CoworkingCenters = coworkingCenters
        };
            
        return View(model);
    }

    [Authorize]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        return View();
    }

    [HttpGet("privacy")]
    public async Task<IActionResult> Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}