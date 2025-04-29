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

[Authorize]
[Route("reservation")]
public class ReservationController
    (
        IReservationService reservationService,
        IReservationRepository reservationRepository,
        IWorkspaceService workspaceService,
        ICoworkingCenterService coworkingCenterService,
        IWorkspaceHistoryRepository workspaceHistoryRepository
    ) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await GetReservations());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationDetailViewModel>> Detail(int id)
    {
        try
        {
            var reservation = (await reservationRepository.GetReservations(new ReservationsFilter
            {
                Id = id, 
                IncludeWorkspace = true,
                IncludeWorkspacePricing = true,
            })).Single();

            var workspace = await workspaceService.GetWorkspaceById(reservation.WorkspaceId);

            return View(new ReservationDetailViewModel { 
                Reservation = reservation,
                Workspace = workspace,
            });
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create
        (
            [FromQuery] int workspaceId,
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null
        )
    {
        var workspaces = await workspaceService.GetWorkspaces(new ());
        
        return View(new ReservationCreateGetViewModel
        {
            Workspaces = workspaces,
            Request = new ReservationCreateRequestDto
            { 
                WorkspaceId = workspaceId,
                StartTime = startTime ?? DateTime.Now.AddHours(1),
                EndTime = endTime ?? DateTime.Now.AddDays(1),
            },
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ReservationCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var workspaces = await workspaceService.GetWorkspaces(new ());
            
            return View(new ReservationCreateGetViewModel 
            {
                Workspaces = workspaces,
                Request = request,
            });
        }
        
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            var reservation = await reservationService.CreateReservation(userId, request);
            return RedirectToAction(nameof(Detail), new { id = reservation.ReservationId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var workspaces = await workspaceService.GetWorkspaces(new WorkspaceQueryRequestDto());

            return View(new ReservationCreateGetViewModel 
            {
                Workspaces = workspaces,
                Request = request,
            });
        }
    }

    [HttpGet("{id:int}/edit")]
    public async Task<ActionResult<Reservation>> Edit(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);
            var workspace = await workspaceService.GetWorkspaceById(reservation.WorkspaceId);
            var histories = await workspaceHistoryRepository.GetHistories(new WorkspaceHistoryFilter { WorkspaceId = workspace.WorkspaceId, IncludeStatus = true });
            var reservations = await reservationRepository.GetReservations(new ReservationsFilter { WorkspaceId = workspace.WorkspaceId });
            var center = await coworkingCenterService.GetCenterById(workspace.CoworkingCenterId);
            var segments = TimelineData.ComputeTimelineSegments(reservations, GetUserId(), out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);

            return View(new ReservationEditViewModel
            {
                Request = new ReservationUpdateRequestDto
                {
                    ReservationId = reservation.ReservationId,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                },
                Reservations = reservations.OrderBy(x => x.StartTime),
                TimelineSegments = segments,
                TimelineStart = timelineStartTime,
                TimelineEnd = timelineEndTime,
                TotalHours = totalHours,
                Workspace = workspace,
                LatestWorkspaceHistory = histories.MaxBy(x => x.ChangeAt),
                PricePerHour = workspace.WorkspacePricings.MaxBy(x => x.ValidFrom)!.PricePerHour,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.TryParseToInt() ?? -1,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/edit")]
    public async Task<ActionResult<Reservation>> Edit(int id, ReservationUpdateRequestDto request)
    {
        try
        {
            var reservation = await reservationService.UpdateReservation(id, request);
            return RedirectToAction(nameof(Detail), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var reservation = await reservationService.GetReservationById(id);

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

            var segments = TimelineData.ComputeTimelineSegments(reservations, GetUserId(), out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);


            return View(new ReservationEditViewModel
            {
                Request = new ReservationUpdateRequestDto
                {
                    ReservationId = reservation.ReservationId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                },
                Reservations = reservations.OrderBy(x => x.StartTime),
                TimelineSegments = segments,
                TimelineStart = timelineStartTime,
                TimelineEnd = timelineEndTime,
                TotalHours = totalHours,
                Workspace = workspace,
                LatestWorkspaceHistory = histories.MaxBy(x => x.ChangeAt),
                PricePerHour = workspace.WorkspacePricings.MaxBy(x => x.ValidFrom)!.PricePerHour,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.TryParseToInt() ?? -1,
            });
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await reservationService.CancelReservation(id);
            return RedirectToAction("Index", ModelState);
        }
        catch (ReservationAlreadyTakingPlaceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    private async Task<IEnumerable<Reservation>> GetReservations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var reservations = await reservationRepository.GetReservations(new ReservationsFilter() 
        {
            CustomerId = userId,
            IncludeWorkspace = true,
            IsCancelled = false,
        });
        
        return reservations;
    }

    private int GetUserId() => User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.TryParseToInt() ?? -1;
}