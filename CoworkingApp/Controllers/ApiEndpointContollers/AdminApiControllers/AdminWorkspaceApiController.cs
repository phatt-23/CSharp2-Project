using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.AdminApiControllers;

public interface IAdminWorkspaceApi
{
    Task<ActionResult<IEnumerable<AdminWorkspaceDto>>>  GetWorkspaces([FromQuery] AdminWorkspaceQueryRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>>               CreateWorkspace([FromBody] WorkspaceCreateRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>>               UpdateWorkspaceDetail(int workspaceId, [FromBody] WorkspaceUpdateRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>>               ChangeWorkspaceStatus(int workspaceId, [FromQuery] WorkspaceStatusType statusType);
    Task<ActionResult<AdminWorkspaceDto>>               DeleteWorkspace(int workspaceId);
}


[AdminApiController]
[Route("api/admin/workspace")]
public class AdminWorkspaceApiController
    (
        IWorkspaceService workspaceService, 
        IMapper mapper
    ) 
    : Controller, IAdminWorkspaceApi
{
    [HttpGet]
    [Authorize(Roles="Admin")]
    public async Task<ActionResult<IEnumerable<AdminWorkspaceDto>>> GetWorkspaces([FromQuery] AdminWorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceService.GetWorkspacesForAdmin(request);
        
        var workspaceDtos = mapper.Map<IEnumerable<WorkspaceDto>>(workspaces);
        
        return Ok(workspaceDtos);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspaceDto>> CreateWorkspace([FromBody] WorkspaceCreateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var workspace = await workspaceService.CreateWorkspace(request);
            return Ok(mapper.Map<WorkspaceDto>(workspace));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspaceDto>> UpdateWorkspaceDetail(int id, [FromBody] WorkspaceUpdateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var workspace = await workspaceService.UpdateWorkspace(id, request);
            return Ok(mapper.Map<AdminWorkspaceDto>(workspace));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspaceDto>> ChangeWorkspaceStatus(int id, [FromQuery]WorkspaceStatusType statusType)
    {
        try
        {
            if (await workspaceService.UpdateWorkspaceStatus(id, statusType))
                return Ok(new { message = "Workspace status changed." });
            
            return BadRequest(new { message = "Unable to change workspace status." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspaceDto>> DeleteWorkspace(int id)
    {
        try
        {
            var workspace = await workspaceService.RemoveWorkspaceById(id);
            return Ok(mapper.Map<AdminWorkspaceDto>(workspace));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}