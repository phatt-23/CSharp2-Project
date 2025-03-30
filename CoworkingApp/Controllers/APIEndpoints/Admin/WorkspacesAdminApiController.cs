using AutoMapper;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

[ApiController]
[Route("api/admin/workspaces")]
public class WorkspacesAdminApiController(
   WorkspacesService workspacesService, 
   IMapper mapper
   ) : Controller
{
   /// ADMIN - Creating a workspace. 
   [Authorize(Roles = "Admin")]
   [HttpPost] 
   public async Task<IActionResult> Create([FromBody] WorkspaceCreateRequestDto request)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      var workspace = await workspacesService.CreateAsync(request);
      return Ok(mapper.Map<WorkspaceDetailDto>(workspace));
   }
   
   
}