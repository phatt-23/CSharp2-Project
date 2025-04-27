// using CoworkingApp.Models.DTOModels;

using CoworkingApp.Models.DTOModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Services;
using CoworkingApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Controllers.MVC;

public interface ICoworkingCenterController
{
    Task<IActionResult> Index(PaginationRequestDto pagination);
    Task<IActionResult> Detail(int id);
    Task<IActionResult> Create(CoworkingCenterCreateRequestDto request);
}

[Route("coworking-center")]
public class CoworkingCenterController
    (
    ICoworkingCenterService coworkingCenterService, 
    IWorkspaceRepository workspaceRepository
    ) 
    : Controller, ICoworkingCenterController 
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationRequestDto pagination)
    {
        var centers = await coworkingCenterService.GetCoworkingCentersAsync(new ());

        return View(new CoworkingCenterIndexViewModel {
            CoworkingCenters = centers,
            Pagination = pagination,
        });
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var center = await coworkingCenterService.GetCoworkingCenterByIdAsync(id);

            var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter
            {
                CoworkingCenterId = center.CoworkingCenterId,
                IncludeLatestPricing = true,
                IncludeStatus = true,
            });

            center.Workspaces = [..workspaces];

            return View(center);
        }
        catch (Exception ex)
        {
            return NotFound($"Coworking center with id of '{id}' wasn't found: {ex.Message}");
        }
    }
    
    [HttpGet("create")] 
    public async Task<IActionResult> Create()
    {
        return View(new CoworkingCenterCreateRequestDto());
    }
    
    [HttpPost("create")] 
    public async Task<IActionResult> Create(CoworkingCenterCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var center = await coworkingCenterService.CreateCoworkingCenterAsync(request);
        return RedirectToAction("Detail", new { id = center.CoworkingCenterId });
    }
}