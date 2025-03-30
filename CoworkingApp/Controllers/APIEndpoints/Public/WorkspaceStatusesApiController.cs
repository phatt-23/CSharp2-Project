using AutoMapper;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

[ApiController]
[Route("/api/workspace-statuses")]
public class WorkspaceStatusesApiController(
    WorkspaceStatusesService workspaceStatusesService,
    IMapper mapper 
    ) : Controller
{
    [HttpGet]
    public async Task<ActionResult<WorkspaceStatusesResponseDto>> GetAsync([FromQuery] WorkspaceStatusQueryDto query)
    {
        var (statuses, totalCount) = await workspaceStatusesService.GetAsync(query);

        var response = new WorkspaceStatusesResponseDto()
        {
            TotalCount = totalCount,
            WorkspaceStatuses = statuses.Select(mapper.Map<WorkspaceStatusDto>).ToList()
        };
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var status = await workspaceStatusesService.GetByIdAsync(id);
        if (status is null)
            return NotFound($"Status with id '{id}' not found");
        
        return Ok(mapper.Map<WorkspaceStatusDto>(status));
    }
}