using System.Security.Claims;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Misc;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

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
    public async Task<IActionResult> Index(
        [FromQuery] PaginationRequestDto pagination,
        [FromQuery] WorkspaceSort sort = WorkspaceSort.None)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
            IncludePricings = true,
            IncludeHistories = true,
            IsRemoved = false,
            Sort = sort,
        });

        return View(new WorkspaceIndexViewModel
        {
            Workspaces = Pagination.Paginate(workspaces, out int totalCount, pagination.PageNumber, pagination.PageSize),
            Pagination = pagination,
            TotalCount = totalCount,
            Sort = sort,
        });
    }

    [HttpGet]
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

            var userId = User.GetUserId() ?? -1;

            var timeline = new TimelineData(workspace, reservations, userId);  

            return View(new WorkspaceDetailViewModel
            {
                Histories = histories,
                CoworkingCenter = center,
                LatestWorkspaceHistory = workspace.GetCurrentHistory(),
                Timeline = timeline,
                PricePerHour = workspace.GetCurrentPricePerHour(),
            });
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Reserve(int id, DateTime? startTime, DateTime? endTime)
    {
        var workspace = await workspaceService.GetWorkspaceById(id);
        if (workspace.GetCurrentStatus().Type != WorkspaceStatusType.Available)
        {
            return RedirectToAction(nameof(Detail), new { id });
        }

        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = workspace.WorkspaceId,
        });

        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized("User not authorized.");
        }

        var timeline = new TimelineData(workspace, reservations, userId.Value);  

        return View(new WorkspaceReserveViewModel
        {
            Workspace = workspace,
            Timeline = timeline,
            Request = new ReservationCreateRequestDto
            {
                WorkspaceId = workspace.WorkspaceId,
                StartTime = (startTime != null) ? startTime.Value : DateTime.Now.AddHours(1),
                EndTime = (endTime != null) ? endTime.Value : DateTime.Now.AddDays(1),
            },
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Reserve(int id, ReservationCreateRequestDto request) 
    {
        var workspace = await workspaceService.GetWorkspaceById(id);
        if (workspace.GetCurrentStatus().Type != WorkspaceStatusType.Available)
        {
            return RedirectToAction(nameof(Detail), new { id });
        }

        if (!ModelState.IsValid) 
        {
            goto loopBack;
        }

        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized("User not authorized.");
        }

        try
        {
            var reservation = await reservationService.CreateReservation(userId.Value, request);
            return RedirectToAction("Detail", "Reservation", new { id = reservation.ReservationId });
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            goto loopBack;
        }

    loopBack:
        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = workspace.WorkspaceId,
        });

        var timeline = new TimelineData(workspace, reservations, -1);

        return View(new WorkspaceReserveViewModel
        {
            Workspace = workspace,
            Request = request,
            Timeline = timeline,
        });
    }
}
