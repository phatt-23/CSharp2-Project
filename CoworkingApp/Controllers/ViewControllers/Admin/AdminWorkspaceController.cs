using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoworkingApp.Controllers.ViewControllers.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminWorkspaceController
    (
        CoworkingCenterService coworkingCenterService,
        WorkspaceStatusService workspaceStatusService,
        WorkspaceService workspaceService
    ) 
    : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var coworkingCenters = await coworkingCenterService.GetCenters(new CoworkingCenterQueryRequestDto());
        var workspaceStatuses = await workspaceStatusService.GetStatuses(new WorkspaceStatusQueryRequestDto());

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
            var coworkingCenters = await coworkingCenterService.GetCenters(new CoworkingCenterQueryRequestDto());
            var workspaceStatuses = await workspaceStatusService.GetStatuses(new WorkspaceStatusQueryRequestDto());

            ViewBag.CoworkingCenters = new SelectList(coworkingCenters, "Id", "Name");
            ViewBag.Statuses = new SelectList(workspaceStatuses, "Id", "Name");

            return View(workspaceCreateRequestDto);
        }

        var createdWorkspace = await workspaceService.CreateWorkspace(workspaceCreateRequestDto);
        return RedirectToAction("Detail", new { id = createdWorkspace.WorkspaceId });
    }

    [Authorize]
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var workspace = await workspaceService.GetWorkspaceById(id);
        return View(workspace);
    }


    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remove(int id)
    {
        try
        {
            _ = await workspaceService.RemoveWorkspaceById(id);
            return RedirectToAction("Index");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("status/{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id)
    {
        return View();
    }

    [HttpGet("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        return View();
    }
}
