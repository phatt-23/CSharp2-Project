using AutoMapper;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.DTOModels.WorkspaceHistory;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;


[ApiController]
[Route("/api/[controller]")]
public class WorkspacesApiController(
   WorkspacesService workspacesService,
   IMapper mapper
   ) : Controller
{
   /// PUBLIC - Get filtered workspaces.
   [HttpGet] 
   public async Task<IActionResult> Get([FromQuery] WorkspacesQueryDto queryDto)
   {
      var (workspaces, totalCount) = await workspacesService.GetAsync(queryDto);

      var response = new WorkspacesResponseDto()
      {
         PageNumber = queryDto.PageNumber,
         PageSize = queryDto.PageSize,
         TotalCount = totalCount,
         Workspaces = workspaces.Select(mapper.Map<WorkspaceDto>)
      };
      
      return Ok(response);
   }

   /// PUBLIC - Get a workspace by id.
   [HttpGet("{id:int}")]
   public async Task<ActionResult<WorkspaceDetailDto?>> GetById(int id)
   {
      var workspace = await workspacesService.GetByIdAsync(id);
      if (workspace is null)
         return NotFound($"Workspace with id '{id}' not found");
      
      return Ok(mapper.Map<WorkspaceDetailDto>(workspace));
   }


   /// PUBLIC - Fetch the status history of a workspace.
   [HttpGet("{id:int}/history")]
   public async Task<IActionResult> GetStatusHistory(int id)
   {
      var workspace = await workspacesService.GetByIdAsync(id);
      if (workspace is null)
         return NotFound($"Workspace with given id of '{id}' doesn't exist");

      var histories = workspacesService.GetHistoriesOfWorkspace(id);

      var response = new WorkspaceHistoriesResponseDto()
      { 
         Workspace = mapper.Map<WorkspaceDto>(workspace),
         Histories = histories.Select(h => new WorkspaceHistoryDto()
         {
            WorkspaceId = workspace.Id,
            StatusId  = workspace.Status.Id,
            CreatedAt = h.CreatedAt,
         }).ToList(),
      };
      
      return Ok(response);
   }


}
