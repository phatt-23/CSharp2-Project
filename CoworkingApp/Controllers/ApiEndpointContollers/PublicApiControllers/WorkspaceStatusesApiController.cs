using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.PublicApiControllers;

public interface IWorkspaceStatusesApi
{
    Task<ActionResult<WorkspaceStatusesResponseDto>> GetAsync([FromQuery] WorkspaceStatusQueryRequestDto request);
    Task<IActionResult> GetByIdAsync(int id);
} 


[ApiController]
[Route("/api/workspace-status")]
public class WorkspaceStatusesApiController
    (
        IWorkspaceStatusService workspaceStatusService,
        IMapper mapper 
    ) 
    : Controller, IWorkspaceStatusesApi
{
    [HttpGet]
    public async Task<ActionResult<WorkspaceStatusesResponseDto>> GetAsync([FromQuery] WorkspaceStatusQueryRequestDto request)
    {
        var statuses = await workspaceStatusService.GetStatuses(request);

        var response = new WorkspaceStatusesResponseDto
        {
            TotalCount = statuses.Count(),
            WorkspaceStatuses = mapper.Map<IEnumerable<WorkspaceStatusDto>>(statuses)
        };
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var status = await workspaceStatusService.GetStatusById(id);
            var statusDto = mapper.Map<WorkspaceStatusDto>(status);
            return Ok(statusDto);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
