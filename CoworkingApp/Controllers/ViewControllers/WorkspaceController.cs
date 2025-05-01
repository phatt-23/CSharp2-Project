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

            var segments = TimelineData.ComputeTimelineSegments(reservations, userId,  
                out double totalHours, 
                out DateTime timelineStartTime, 
                out DateTime timelineEndTime);

            return View(new WorkspaceDetailViewModel
            {
                Workspace = workspace,
                Histories = histories,
                CoworkingCenter = center,
                Reservations = reservations.OrderBy(x => x.StartTime),
                TimelineSegments = segments,
                TimelineStart = timelineStartTime,
                TimelineEnd = timelineEndTime,
                TotalHours = totalHours,
                LatestWorkspaceHistory = workspace.GetCurrentHistory(),
                PricePerHour = workspace.GetCurrentPricePerHour(),
                UserId = User.GetUserId() ?? -1,
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
        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = workspace.WorkspaceId,
        });

        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized("User not authorized.");
        }

        var segments = TimelineData.ComputeTimelineSegments(reservations, userId.Value,  
            out double totalHours, 
            out DateTime timelineStartTime, 
            out DateTime timelineEndTime);

        return View(new WorkspaceReserveViewModel
        {
            Workspace = workspace,
            Reservations = reservations.OrderBy(x => x.StartTime),
            TimelineSegments = segments,
            TotalHours = totalHours,
            TimelineStart = timelineStartTime,
            TimelineEnd = timelineEndTime,
            PricePerHour = workspace.GetCurrentPricePerHour(),
            UserId = User.GetUserId() ?? -1,
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
        var workspace = await workspaceService.GetWorkspaceById(id);
        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = workspace.WorkspaceId,
        });

        var segments = TimelineData.ComputeTimelineSegments(reservations, -1,  out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);

        return View(new WorkspaceReserveViewModel
        {
            Workspace = workspace,
            Reservations = reservations.OrderBy(x => x.StartTime),
            UserId = -1,
            Request = request,
            PricePerHour = workspace.GetCurrentPricePerHour(),
            TimelineSegments = segments,
            TotalHours = totalHours,
            TimelineStart = timelineStartTime,
            TimelineEnd = timelineEndTime,
        });
    }
}
