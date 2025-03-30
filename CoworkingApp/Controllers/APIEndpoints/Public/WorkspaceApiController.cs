using AutoMapper;
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
[Route("/api/[controller]")]
public class WorkspaceApiController(
    IWorkspaceService workspacesService,
    IPaginationService paginationService,
    IMapper mapper
    ) : Controller, IWorkspaceApi
{
   /// PUBLIC - Get filtered workspaces.
    [HttpGet] 
    public async Task<ActionResult<IEnumerable<WorkspaceDto>>> GetWorkspacesAsync([FromQuery] WorkspaceQueryRequestDto request)
   {
      var workspaces = await workspacesService.GetWorkspacesAsync(request);
      
      var workspacePage = paginationService.Paginate(workspaces, request.PageNumber, request.PageSize);
      var workspaceDtos = mapper.Map<IEnumerable<WorkspaceDto>>(workspacePage);
      var totalCount = workspaces.Count(); 
      
      var response = new WorkspacesResponseDto
      {
         PageNumber = request.PageNumber,
         PageSize = request.PageSize,
         TotalCount = totalCount,
         Workspaces = workspaceDtos
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
            var histories = workspacesService.GetWorkspaceHistoryAsync(id);

            var response = new WorkspaceHistoriesResponseDto
            { 
                Workspace = mapper.Map<WorkspaceDto>(workspace),
                Histories = mapper.Map<IEnumerable<WorkspaceHistoryDto>>(histories)
            };
            
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


}
