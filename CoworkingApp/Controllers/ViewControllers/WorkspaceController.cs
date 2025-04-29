using System.Security.Claims;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Misc;
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
    public async Task<IActionResult> Index(
        [FromQuery] PaginationRequestDto pagination,
        [FromQuery] WorkspaceSort sort = WorkspaceSort.None)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
            IncludePricings = true,
            IsRemoved = false,
            Sort = sort,
        });
        
        return View(new WorkspaceIndexViewModel
        {
            Workspaces = workspaces,
            Pagination = pagination,
            Sort = sort,
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

            var segments = TimelineData.ComputeTimelineSegments(reservations, GetUserId(),  out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);

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
                LatestWorkspaceHistory = histories.MaxBy(x => x.ChangeAt),
                PricePerHour = workspace.WorkspacePricings.MaxBy(x => x.ValidFrom)!.PricePerHour,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.TryParseToInt() ?? -1,
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
        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = workspace.WorkspaceId,
        });

        var segments = TimelineData.ComputeTimelineSegments(reservations, GetUserId(),  out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);

        return View(new WorkspaceReserveViewModel
        {
            Workspace = workspace,
            Reservations = reservations.OrderBy(x => x.StartTime),
            TimelineSegments = segments,
            TotalHours = totalHours,
            TimelineStart = timelineStartTime,
            TimelineEnd = timelineEndTime,
            PricePerHour = workspace.WorkspacePricings.MaxBy(x => x.ValidFrom)!.PricePerHour,
            UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
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
            goto loopBack;
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

            goto loopBack;
        }

    loopBack:
        var workspace = await workspaceService.GetWorkspaceById(id);
        var reservations = await reservationRepository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = workspace.WorkspaceId,
        });

        var segments = TimelineData.ComputeTimelineSegments(reservations, GetUserId(),  out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);

        return View(new WorkspaceReserveViewModel
        {
            Workspace = workspace,
            Reservations = reservations.OrderBy(x => x.StartTime),
            TimelineSegments = segments,
            TotalHours = totalHours,
            TimelineStart = timelineStartTime,
            TimelineEnd = timelineEndTime,
            UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
            PricePerHour = workspace.WorkspacePricings.MaxBy(x => x.ValidFrom)!.PricePerHour,
            Request = request,
        });
    }


    private int GetUserId() => User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.TryParseToInt() ?? -1;

}
