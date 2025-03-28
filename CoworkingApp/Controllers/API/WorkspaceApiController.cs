using System.Text.Json;
using System.Text.Json.Serialization;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.API;


[ApiController]
[Route("/api/workspaces")]
public class WorkspaceApiController(WorkspacesService workspacesService) : Controller
{
   private readonly WorkspacesService _workspacesService = workspacesService;

   // Fetching all workspaces (GET /api/workspaces)
   // Getting a specific workspace (GET /api/workspaces/{id})
   // Creating a workspace (POST /api/workspaces)
   // Updating workspace status (PUT /api/workspaces/{id}/status)
   // Viewing workspace status history (GET /api/workspaces/{id}/history) 

   /// Fetching all workspaces (GET /api/workspaces?)
   [HttpGet] 
   public async Task<IActionResult> GetWorkspaces([FromQuery] WorkspacesQueryDto queryDto)
   {
      var (workspaces, totalCount) = await _workspacesService.GetAsync(queryDto);

      var response = new
      {
         PageNumber = queryDto.Page,
         queryDto.PageSize,
         TotalCount = totalCount,
         Workspaces = workspaces,
      };
      
      return Ok(response);
   }

   /// Creating a workspace (POST /api/workspaces)
   [HttpPost] 
   public async Task<IActionResult> CreateWorkspace([FromBody] WorkspaceCreateRequestDto workspaceCreateRequestDtoRequest)
   {
      // create workspace and return the created workspace's id
      var workspace = await _workspacesService.CreateAsync(workspaceCreateRequestDtoRequest);
      return Ok(workspace.Id);
   }

   /// Updating workspace status (PUT /api/workspaces/{id}/status)
   [HttpPut("{workspaceId:int}/status")]
   public async Task<IActionResult> UpdateWorkspace(int workspaceId, [FromBody] int statusId)
   {
      var isUpdated = await _workspacesService.UpdateStatusAsync(workspaceId, statusId);
      if (!isUpdated)
      {
         return NotFound("Workspace or status could not be found");
      }
      return NoContent();
   }
}