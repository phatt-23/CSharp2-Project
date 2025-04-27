using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;


public interface IAdminWorkspaceApi
{
    Task<ActionResult<IEnumerable<AdminWorkspaceDto>>> GetWorkspacesAsync([FromQuery] AdminWorkspaceQueryRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>> CreateWorkspaceAsync([FromBody] WorkspaceCreateRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>> UpdateWorkspaceDetailAsync(int workspaceId, [FromBody] WorkspaceUpdateRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>> ChangeWorkspaceStatusAsync(int workspaceId, [FromQuery] WorkspaceStatusType statusType);
    Task<ActionResult<AdminWorkspaceDto>> DeleteWorkspaceAsync(int workspaceId);
}


[ApiController]
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
    public async Task<ActionResult<IEnumerable<AdminWorkspaceDto>>> GetWorkspacesAsync([FromQuery] AdminWorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceService.GetWorkspacesForAdminAsync(request);
        
        var workspaceDtos = mapper.Map<IEnumerable<WorkspaceDto>>(workspaces);
        
        return Ok(workspaceDtos);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspaceDto>> CreateWorkspaceAsync([FromBody] WorkspaceCreateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var workspace = await workspaceService.CreateWorkspaceAsync(request);
            return Ok(mapper.Map<WorkspaceDto>(workspace));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspaceDto>> UpdateWorkspaceDetailAsync(int id, [FromBody] WorkspaceUpdateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var workspace = await workspaceService.UpdateWorkspaceAsync(id, request);
            var workspaceDto = mapper.Map<AdminWorkspaceDto>(workspace);
            return Ok(workspaceDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<AdminWorkspaceDto>> ChangeWorkspaceStatusAsync(int id, [FromQuery]WorkspaceStatusType statusType)
    {
        try
        {
            if (await workspaceService.UpdateWorkspaceStatusAsync(id, statusType))
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
    public async Task<ActionResult<AdminWorkspaceDto>> DeleteWorkspaceAsync(int id)
    {
        try
        {
            var workspace = await workspaceService.RemoveWorkspaceByIdAsync(id);
            return Ok(mapper.Map<AdminWorkspaceDto>(workspace));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}