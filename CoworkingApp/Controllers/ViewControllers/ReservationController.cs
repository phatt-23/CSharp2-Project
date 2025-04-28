using System.Security.Claims;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
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
            
            return View(new ReservationCreateGetViewModel {
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
            await reservationService.CancelReservation(id);
            return RedirectToAction("Index", ModelState);
        }
        catch (ReservationAlreadyTakingPlaceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", await GetReservations());
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
}