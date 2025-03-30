using System.Text;
using System.Text.Json;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoworkingApp.Controllers.MVC;

/// For the end user in the browser
public class WorkspaceController(
    IWorkspaceService workspaceService,
    ICoworkingCenterService coworkingCenterService,
    IWorkspaceStatusService workspaceStatusService
    ) : Controller
{
    
    /// Workspace listing
    /// GET workspaces/
    [HttpGet]
    public async Task<IActionResult> Index(WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceService.GetWorkspacesAsync(request);
        return View(workspaces);
    }

    
    /// Show workspace detail
    /// GET workspaces/{id}
    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        // total count should always be 1
        try
        {
            var workspace = await workspaceService.GetWorkspaceByIdAsync(id);
            return View(workspace);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    /// Show create form
    /// GET workspaces/create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var coworkingCenters = await coworkingCenterService.GetCoworkingCentersAsync(new CoworkingCenterQueryRequestDto());
        var workspaceStatuses = await workspaceStatusService.GetWorkspaceStatusesAsync(new WorkspaceStatusQueryRequestDto());
        
        ViewBag.CoworkingCenters = new SelectList(coworkingCenters, "Id", "Name");
        ViewBag.Statuses = new SelectList(workspaceStatuses, "Id", "Name");
        
        return View(new WorkspaceCreateRequestDto());
    }
    
    
    /// Handle submitted form for creation of a new workspace
    /// GET workspaces/create
    [HttpPost]
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
        return RedirectToAction("Detail", new { id = createdWorkspace.Id });
    }


    /// GET /workspaces/edit/{id}
    [HttpGet] 
    public async Task<IActionResult> Edit(int id)
    {
        var workspace = await workspaceService.GetWorkspaceByIdAsync(id);
        return View(workspace);
    }


    [HttpDelete]
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
