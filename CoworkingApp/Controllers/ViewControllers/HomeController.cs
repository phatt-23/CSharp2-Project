using System.Diagnostics;
using System.Security.Claims;
using CoworkingApp.Models;
using CoworkingApp.Models.Misc;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

public class HomeController
    (
        IWorkspaceRepository workspaceRepository,
        ICoworkingCenterRepository coworkingCenterRepository,
        IReservationRepository reservationRepository,
        IUserRepository userRepository
    ) 
    : Controller
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

        var coworkingCenters = await coworkingCenterRepository.GetCenters(new CoworkingCenterFilter());

        return View(new HomeIndexViewModel()
        {
            Workspaces = workspaces,
            CoworkingCenters = coworkingCenters
        });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Dashboard([FromQuery] ReservationSort reservationSort = ReservationSort.None)
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
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
            User = user,
            Reservations = reservations,
            ReservationSort = reservationSort,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}