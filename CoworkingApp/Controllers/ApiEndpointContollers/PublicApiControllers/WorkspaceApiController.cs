using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.PublicApiControllers;

internal interface IWorkspaceApi
{
    Task<ActionResult<IEnumerable<WorkspaceDto>>> GetWorkspaces([FromQuery] WorkspaceQueryRequestDto request);
    Task<ActionResult<WorkspaceDto?>> GetWorkspacesById(int id);
}


[PublicApiController]
[Route("/api/workspace")]
public class WorkspaceApiController
    (
        IWorkspaceService workspacesService,
        IMapper mapper
    ) 
    : Controller, IWorkspaceApi
{
    /// PUBLIC - Get filtered workspaces.
    [HttpGet] 
    public async Task<ActionResult<IEnumerable<WorkspaceDto>>> GetWorkspaces([FromQuery] WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspacesService.GetWorkspaces(request);
        
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
    public async Task<ActionResult<WorkspaceDto?>> GetWorkspacesById(int id)
    { 
        try
        {
            var workspace = await workspacesService.GetWorkspaceById(id);
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
            var workspace = await workspacesService.GetWorkspaceById(id);
            var histories = await workspacesService.GetWorkspaceHistory(id);

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
