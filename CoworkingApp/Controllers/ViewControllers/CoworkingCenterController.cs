using CoworkingApp.Services;
using CoworkingApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Controllers.ViewControllers;

public class CoworkingCenterController
    (
        ICoworkingCenterService coworkingCenterService,
        ICoworkingCenterRepository coworkingCenterRepository,
        IWorkspaceRepository workspaceRepository
    ) 
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationRequestDto pagination)
    {
        var centers = await coworkingCenterRepository.GetCenters(new CoworkingCenterFilter
        {
            IncludeWorkspaces = true,
            IncludeAddress = true,
        });

        return View(new CoworkingCenterIndexViewModel 
        {
            CoworkingCenters = Pagination.Paginate(centers, out int totalCount, pagination.PageNumber, pagination.PageSize),
            Pagination = pagination,
            TotalCount = totalCount,
        });
    }
    
    [HttpGet]
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
}