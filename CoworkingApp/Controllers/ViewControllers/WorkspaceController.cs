using System.Security.Claims;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

[Route("workspace")]
public class WorkspaceController
    (
        IWorkspaceService workspaceService,
        IWorkspaceRepository workspaceRepository,
        ICoworkingCenterService coworkingCenterService,
        IWorkspaceHistoryRepository workspaceHistoryRepository,
        IReservationRepository reservationRepository,
        IReservationService reservationService
    ) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationRequestDto pagination)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
            IncludeLatestPricing = true,
            IsRemoved = false,
        });
        
        return View(new WorkspaceIndexViewModel()
        {
            Workspaces = workspaces,
            Pagination = pagination,
        });
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var workspace = await workspaceService.GetWorkspaceById(id);

            var histories = await workspaceHistoryRepository.GetHistories(new WorkspaceHistoryFilter 
            {
                WorkspaceId = workspace.WorkspaceId,
                IncludeStatus = true,
            });

            var reservations = await reservationRepository.GetReservations(new ReservationsFilter 
            {
                WorkspaceId = workspace.WorkspaceId,
            });

            var center = await coworkingCenterService.GetCenterById(workspace.CoworkingCenterId);

            return View(new WorkspaceDetailViewModel 
            { 
                Workspace = workspace, 
                Histories = histories, 
                CoworkingCenter = center,
                Reservations = reservations, 
            });
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id:int}/reserve")]
    public async Task<IActionResult> Reserve(int id, DateTime? startTime, DateTime? endTime)
    {
        var workspace = await workspaceService.GetWorkspaceById(id);
        var reservations = await reservationRepository.GetReservations(new ());

        return View(new WorkspaceReserveViewModel 
        {
            Workspace = workspace,
            Reservations = reservations,
            Request = new ReservationCreateRequestDto 
            { 
                WorkspaceId = workspace.WorkspaceId,
                StartTime = (startTime != null) ? startTime.Value : DateTime.Now.AddHours(1),
                EndTime = (endTime != null) ? endTime.Value : DateTime.Now.AddDays(1),
            },
        });
    }

    [Authorize]
    [HttpPost("{id:int}/reserve")]
    public async Task<IActionResult> Reserve(int id, ReservationCreateRequestDto request) 
    {
        if (!ModelState.IsValid) 
        {
            return View(new WorkspaceReserveViewModel 
            {
                Request = request,
                Workspace = await workspaceService.GetWorkspaceById(id),
                Reservations = await reservationRepository.GetReservations(new()),
            });
        }

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var reservation = await reservationService.CreateReservation(userId, request);
            return RedirectToAction("Detail", "Reservation", new { id = reservation.ReservationId });
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = ex.Message;

            return View(new WorkspaceReserveViewModel
            {
                Request = request,
                Workspace = await workspaceService.GetWorkspaceById(id),
                Reservations = await reservationRepository.GetReservations(new()),
            });
        }

    }
}
