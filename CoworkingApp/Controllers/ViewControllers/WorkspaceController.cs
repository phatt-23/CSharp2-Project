using System.Security.Claims;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Models.ViewModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CoworkingApp.Controllers.MVC;

/// For the end user in the browser
[Route("workspace")]
public class WorkspaceController
    (
    IWorkspaceService workspaceService,
    IWorkspaceRepository workspaceRepository,
    ICoworkingCenterService coworkingCenterService,
    IWorkspaceStatusService workspaceStatusService,
    IWorkspaceHistoryRepository workspaceHistoryRepository,
    IReservationRepository reservationRepository,
    IReservationService reservationService
    ) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationRequestDto pagination)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter
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
        // total count should always be 1
        try
        {
            var workspace = await workspaceService.GetWorkspaceByIdAsync(id);
            var histories = await workspaceHistoryRepository.GetHistoriesAsync(new WorkspaceHistoryFilter {
                WorkspaceId = workspace.WorkspaceId,
                IncludeStatus = true,
            });
            var reservations = await reservationRepository.GetReservationsAsync(new ReservationsFilter {
                WorkspaceId = workspace.WorkspaceId,
            });
            var center = await coworkingCenterService.GetCoworkingCenterByIdAsync(workspace.CoworkingCenterId);

            return View(new WorkspaceDetailViewModel { 
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
    public async Task<IActionResult> Reserve
        (
        int id, 
        DateTime? startTime,
        DateTime? endTime
        )
    {
        var workspace = await workspaceService.GetWorkspaceByIdAsync(id);
        var request = new ReservationCreateRequestDto { 
            WorkspaceId = workspace.WorkspaceId 
        };

        if (startTime.HasValue) request.StartTime = startTime.Value;
        if (endTime.HasValue) request.StartTime = endTime.Value;

        var reservations = await reservationRepository.GetReservationsAsync(new ());

        return View(new WorkspaceReserveViewModel {
            Workspace = workspace,
            Request = request,
            Reservations = reservations,
        });
    }

    [Authorize]
    [HttpPost("{id:int}/reserve")]
    public async Task<IActionResult> Reserve(int id, ReservationCreateRequestDto request) {
        if (!ModelState.IsValid) {
            var workspace = await workspaceService.GetWorkspaceByIdAsync(id);
            var reservations = await reservationRepository.GetReservationsAsync(new ());

            return View(new WorkspaceReserveViewModel {
                Workspace = workspace,
                Request = request,
                Reservations = reservations,
            });
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var reservation = await reservationService.CreateReservationAsync(userId, request);
        //  public virtual RedirectToActionResult RedirectToAction(string? actionName, string? controllerName, string? fragment);
        return RedirectToAction("Detail", "Reservation", new { id = reservation.ReservationId });
    }
    

    [Authorize]
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var coworkingCenters = await coworkingCenterService.GetCoworkingCentersAsync(new CoworkingCenterQueryRequestDto());
        var workspaceStatuses = await workspaceStatusService.GetWorkspaceStatusesAsync(new WorkspaceStatusQueryRequestDto());
        
        ViewBag.CoworkingCenters = new SelectList(coworkingCenters, "Id", "Name");
        ViewBag.Statuses = new SelectList(workspaceStatuses, "Id", "Name");
        
        return View(new WorkspaceCreateRequestDto
        {
            Name = string.Empty,
            Description = string.Empty
        });
    }
    
    
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create(WorkspaceCreateRequestDto workspaceCreateRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var coworkingCenters = await coworkingCenterService.GetCoworkingCentersAsync(new CoworkingCenterQueryRequestDto());
            var workspaceStatuses = await workspaceStatusService.GetWorkspaceStatusesAsync(new WorkspaceStatusQueryRequestDto());
            
            ViewBag.CoworkingCenters = new SelectList(coworkingCenters, "Id", "Name");
            ViewBag.Statuses = new SelectList(workspaceStatuses, "Id", "Name");
            
            return View(workspaceCreateRequestDto);
        }

        var createdWorkspace = await workspaceService.CreateWorkspaceAsync(workspaceCreateRequestDto);
        return RedirectToAction("Detail", new { id = createdWorkspace.WorkspaceId });
    }

    [Authorize]
    [HttpGet("{id:int}/edit")] 
    public async Task<IActionResult> Edit(int id)
    {
        var workspace = await workspaceService.GetWorkspaceByIdAsync(id);
        return View(workspace);
    }


    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remove(int id)
    {
        try
        {
            _ = await workspaceService.RemoveWorkspaceByIdAsync(id);
            return RedirectToAction("Index");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    
    
    
    
    
}
