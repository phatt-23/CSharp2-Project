using CoworkingApp.Services;
using CoworkingApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Models.DtoModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CoworkingApp.Controllers.ViewControllers;
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
        ICoworkingCenterRepository coworkingCenterRepository,
        IWorkspaceRepository workspaceRepository,
        IGeocodingService geocodingService
    ) 
    : Controller, ICoworkingCenterController 
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationRequestDto pagination)
    {
        var centers = await coworkingCenterRepository.GetCenters(new ()
        {
            IncludeWorkspaces = true,
            IncludeAddress = true,
        });

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
            var center = await coworkingCenterService.GetCenterById(id);

            var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        return View(new CoworkingCenterCreateRequestDto());
    }
    
    [HttpPost("create")] 
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CoworkingCenterCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var geocode = geocodingService.GeocodeAsync(request.StreetAddress, request.District, request.City, request.PostalCode, request.Country);
        if (geocode == null)
        {
            return BadRequest("The address could not be validated.");
        }

        var center = await coworkingCenterService.CreateCenter(request);

        return RedirectToAction("Detail", new { id = center.CoworkingCenterId });
    }
}