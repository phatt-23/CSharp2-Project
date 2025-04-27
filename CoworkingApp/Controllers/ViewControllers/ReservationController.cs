using System.Security.Claims;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoworkingApp.Controllers.MVC;

[Authorize]
[Route("reservation")]
public class ReservationController
    (
    IReservationService reservationService,
    IReservationRepository reservationRepository,
    IWorkspaceService workspaceService
    ) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await GetReservations());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var reservations = await reservationRepository.GetReservationsAsync(new ReservationsFilter()
            {
                Id = id, 
                IncludeWorkspace = true,
            });
            
            return View(reservations.Single());
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
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime
        )
    {
        var workspaces = await workspaceService.GetWorkspacesAsync(new WorkspaceQueryRequestDto());
        
        return View(new ReservationCreateGetViewModel {
            Workspaces = workspaces,
            Request = new ReservationCreateRequestDto { WorkspaceId = workspaceId, StartTime = startTime, EndTime = endTime },
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ReservationCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var workspaces = await workspaceService.GetWorkspacesAsync(new WorkspaceQueryRequestDto());
            
            return View(new ReservationCreateGetViewModel {
                Workspaces = workspaces,
                Request = request,
            });
        }
        
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            var reservation = await reservationService.CreateReservationAsync(userId, request);
            return RedirectToAction("Detail", new { id = reservation.ReservationId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var workspaces = await workspaceService.GetWorkspacesAsync(new WorkspaceQueryRequestDto());
            ViewBag.Workspaces = new SelectList(workspaces, nameof(Workspace.WorkspaceId), nameof(Workspace.Name));
            return View(new ReservationCreateGetViewModel {
                Workspaces = workspaces,
                Request = request,
            });
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await reservationService.CancelReservationAsync(id);
        }
        catch (ReservationAlreadyTakingPlaceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", await GetReservations());
        }
        
        return RedirectToAction("Index", ModelState);
    }

    private async Task<IEnumerable<Reservation>> GetReservations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var reservations = await reservationRepository.GetReservationsAsync(new ReservationsFilter() 
        {
            CustomerId = userId,
            IncludeWorkspace = true,
        });
        
        return reservations;
    }

}