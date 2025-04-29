using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using CoworkingApp.Models;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

public interface IHomeController
{
    public Task<IActionResult> Index(); 
    public Task<IActionResult> Dashboard([FromQuery] ReservationSort reservationSort = ReservationSort.None); 
}

public class HomeController
    (
        IWorkspaceRepository workspaceRepository,
        ICoworkingCenterRepository coworkingCenterRepository,
        IReservationRepository reservationRepository,
        IUserRepository userRepository
    ) 
    : Controller, IHomeController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new ()
        {
            HasPricing = true,
            IncludePricings = true,
            IncludeStatus = true,
        });

        var coworkingCenters = await coworkingCenterRepository.GetCenters(new CoworkingCenterFilter()
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
    public async Task<IActionResult> Dashboard([FromQuery] ReservationSort reservationSort = ReservationSort.None)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var reservations = await reservationRepository.GetReservations(new ReservationsFilter()
        {
            CustomerId = userId,
            IsCancelled = false,
            IncludeWorkspace = true,
            Sort = reservationSort,
        });

        var user = (await userRepository.GetUsers(new UserFilter
        {
            UserId = userId
        })).Single();

        return View(new HomeDashboardViewModel 
        { 
            Reservations = reservations,
            User = user,
            ReservationSort = reservationSort,
        });
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