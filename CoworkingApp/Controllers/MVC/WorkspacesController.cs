using System.Text;
using System.Text.Json;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoworkingApp.Controllers.MVC;

/// For the end user in the browser
public class WorkspacesController(
    WorkspacesService workspacesService,
    CoworkingCentersService coworkingCentersService,
    WorkspaceStatusesService workspaceStatusesService
    ) : Controller
{
    
    /// Workspace listing
    /// GET workspaces/
    [HttpGet]
    public async Task<IActionResult> Index(WorkspacesQueryDto queryDto)
    {
        var (workspaces, _) = await workspacesService.GetAsync(queryDto);
        
        return View(workspaces);
    }

    
    /// Show workspace detail
    /// GET workspaces/{id}
    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        // total count should always be 1
        var workspace = await workspacesService.GetByIdAsync(id);
        if (workspace is null)
        {
            return NotFound($"The workspace with id '{id}' was not found.");
        }

        return View(workspace);
    }


    /// Show create form
    /// GET workspaces/create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var (coworkingCenters, _) = await coworkingCentersService.GetAsync(new CoworkingCentersQueryDto());
        var (workspaceStatuses, _) = await workspaceStatusesService.GetAsync(new WorkspaceStatusQueryDto());
        
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
            var (coworkingCenters, _) = await coworkingCentersService.GetAsync(new CoworkingCentersQueryDto());
            var (workspaceStatuses, _) = await workspaceStatusesService.GetAsync(new WorkspaceStatusQueryDto());
            
            ViewBag.CoworkingCenters = new SelectList(coworkingCenters, "Id", "Name");
            ViewBag.Statuses = new SelectList(workspaceStatuses, "Id", "Name");
            
            return View(workspaceCreateRequestDto);
        }

        var createdWorkspace = await workspacesService.CreateAsync(workspaceCreateRequestDto);
        return RedirectToAction("Detail", new { id = createdWorkspace.Id });
    }


    /// GET /workspaces/edit/{id}
    [HttpGet] 
    public async Task<IActionResult> Edit(int id)
    {
        var workspace = await workspacesService.GetByIdAsync(id);
        return View(workspace);
    }


    [HttpDelete]
    public async Task<IActionResult> Remove(int id)
    {
        var workspace = await workspacesService.RemoveByIdAsync(id);
        if (workspace is null)
            return NotFound($"The workspace with id '{id}' was not found.");

        return RedirectToAction("Index");
    }
    
    
    
    
    
    
}
