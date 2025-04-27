using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

internal interface IWorkspaceApi
{
    Task<ActionResult<IEnumerable<WorkspaceDto>>> GetWorkspacesAsync([FromQuery] WorkspaceQueryRequestDto request);
    Task<ActionResult<WorkspaceDto?>> GetWorkspacesByIdAsync(int id);
}


[ApiController]
[Route("/api/workspace")]
public class WorkspaceApiController(
    IWorkspaceService workspacesService,
    IMapper mapper
    ) : Controller, IWorkspaceApi
{
    /// PUBLIC - Get filtered workspaces.
    [HttpGet] 
    public async Task<ActionResult<IEnumerable<WorkspaceDto>>> GetWorkspacesAsync([FromQuery] WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspacesService.GetWorkspacesAsync(request);
        
        var totalCount = workspaces.Count(); 
        var workspacePage = Pagination.Paginate(workspaces, request.PageNumber, request.PageSize);
        var workspaceDtos = mapper.Map<IEnumerable<WorkspaceDto>>(workspacePage);
      
        var response = new WorkspacesResponseDto
        {
            TotalCount = totalCount,
            Workspaces = workspaceDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };

        return Ok(response);
    }

    /// PUBLIC - Get a workspace by id.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkspaceDto?>> GetWorkspacesByIdAsync(int id)
    { 
        try
        {
            var workspace = await workspacesService.GetWorkspaceByIdAsync(id);
            var response = mapper.Map<WorkspaceDto>(workspace);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    /// PUBLIC - Fetch the status history of a workspace.
    [HttpGet("{id:int}/history")]
    public async Task<IActionResult> GetStatusHistory(int id)
    {
        try
        {
            var workspace = await workspacesService.GetWorkspaceByIdAsync(id);
            var histories = await workspacesService.GetWorkspaceHistoryAsync(id);

            var workspaceDto = mapper.Map<WorkspaceDto>(workspace);
            var historyDtos = mapper.Map<IEnumerable<WorkspaceHistoryDto>>(histories);
            
            var response = new WorkspaceHistoriesResponseDto
            { 
                Workspace = workspaceDto,
                Histories = historyDtos
            };
            
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


}
