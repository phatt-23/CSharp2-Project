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
    ILogger<WorkspacesController> logger,
    WorkspacesService workspacesService,
    CoworkingCentersService coworkingCentersService,
    WorkspaceStatusesService workspaceStatusesService
    ) : Controller
{
    private readonly WorkspacesService _workspacesService = workspacesService;
    private readonly CoworkingCentersService _coworkingCentersService = coworkingCentersService;
    private readonly WorkspaceStatusesService _workspaceStatusesService = workspaceStatusesService;
    private readonly ILogger _logger = logger;
    
    /// Workspace listing
    [HttpGet]
    public async Task<IActionResult> Index(WorkspacesQueryDto queryDto)
    {
        var (workspaces, _) = await _workspacesService.GetAsync(queryDto);
        
        return View(workspaces);
    }

    
    // Show workspace detail 
    [HttpGet]
    [Route("/workspaces/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        // total count should always be 1
        var (workspaces, _) = await _workspacesService.GetAsync(new WorkspacesQueryDto
        {
            Id = id
        });

        try
        {
            var workspace = workspaces.Single();
            return View(workspace);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"The workspace with id '{id}' was not found.");
        }

    }


    // Create form
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var (coworkingCenters, _) = await _coworkingCentersService.GetAsync(new CoworkingCentersQueryDto());
        var (workspaceStatuses, _) = await _workspaceStatusesService.GetAsync(new WorkspaceStatusQueryDto());
        
        ViewBag.CoworkingCenters = new SelectList(coworkingCenters, "Id", "Name");
        ViewBag.Statuses = new SelectList(workspaceStatuses, "Id", "Name");
        
        return View(new WorkspaceCreateRequestDto());
    }
    
    
    // Handle submitted form for creation of a new workspace
    [HttpPost]
    public async Task<IActionResult> Create(WorkspaceCreateRequestDto workspaceCreateRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Workspace creation failed");
        }

        var createdWorkspace = await _workspacesService.CreateAsync(workspaceCreateRequestDto);

        return RedirectToAction("Detail", new { id = createdWorkspace.Id });
    }


    [HttpGet] 
    [Route("/workspaces/edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var (workspaces, totalCount) = await _workspacesService.GetAsync(
            new WorkspacesQueryDto
            {
                Id = id
            });
        
        var workspace = workspaces.Single();
        
        return View(workspace);
    }

    
    

}
