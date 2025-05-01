using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Models.Misc;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

[Authorize]
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
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        var reservations = await reservationRepository.GetReservations(new ReservationsFilter()
        {
            CustomerId = userId,
            IncludeWorkspace = true,
            IsCancelled = false,
        });

        return View(reservations);
    }

    [HttpGet]
    public async Task<ActionResult<ReservationDetailViewModel>> Detail(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);
            var workspace = await workspaceService.GetWorkspaceById(reservation.WorkspaceId);

            return View(new ReservationDetailViewModel 
            { 
                Reservation = reservation,
                Workspace = workspace,
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create([FromQuery] int workspaceId, [FromQuery] DateTime? startTime = null, [FromQuery] DateTime? endTime = null)
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

    [HttpPost]
    public async Task<IActionResult> Create(ReservationCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            goto defer;
        }

        var userId = User.GetUserId();

        if (userId == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        try
        {
            var reservation = await reservationService.CreateReservation(userId.Value, request);
            return RedirectToAction(nameof(Detail), new { id = reservation.ReservationId });
        }
        catch (FormValidationException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

    defer:
        var workspaces = await workspaceService.GetWorkspaces(new WorkspaceQueryRequestDto());

        return View(new ReservationCreateGetViewModel
        {
            Workspaces = workspaces,
            Request = request,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, [FromBody] ReservationUpdateRequestDto? request = null)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);
            var workspace = await workspaceService.GetWorkspaceById(reservation.WorkspaceId);
            var center = await coworkingCenterService.GetCenterById(workspace.CoworkingCenterId);
            var histories = await workspaceHistoryRepository.GetHistories(new WorkspaceHistoryFilter { WorkspaceId = workspace.WorkspaceId, IncludeStatus = true });
            var reservations = await reservationRepository.GetReservations(new ReservationsFilter { WorkspaceId = workspace.WorkspaceId });

            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var segments = TimelineData.ComputeTimelineSegments(reservations, userId.Value, out double totalHours, out DateTime timelineStartTime, out DateTime timelineEndTime);

            return View(new ReservationEditViewModel
            {
                Request = request ?? new ReservationUpdateRequestDto
                {
                    ReservationId = reservation.ReservationId,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                },
                Workspace = workspace,
                Reservations = reservations.OrderBy(x => x.StartTime),
                LatestWorkspaceHistory = workspace.GetCurrentHistory(),
                PricePerHour = workspace.GetCurrentPricePerHour(),
                UserId = userId.Value,
                TimelineStart = timelineStartTime,
                TimelineEnd = timelineEndTime,
                TotalHours = totalHours,
                TimelineSegments = segments,
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> Edit(ReservationUpdateRequestDto request)
    {
        try
        {
            var reservation = await reservationService.UpdateReservation(request);
            return RedirectToAction(nameof(Detail), new { id = request.ReservationId });
        }
        catch (FormValidationException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
            return RedirectToAction(nameof(Edit), new { id = request.ReservationId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction(nameof(Edit), new { id = request.ReservationId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await reservationService.CancelReservation(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }
}