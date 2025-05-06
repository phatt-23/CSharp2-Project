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
    Task<ActionResult<AdminWorkspacesResponseDto>> GetWorkspaces([FromQuery] AdminWorkspaceQueryRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>> GetWorkspace(int id);
    Task<ActionResult<AdminWorkspaceDto>> CreateWorkspace([FromBody] WorkspaceCreateRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>> UpdateWorkspace([FromBody] WorkspaceUpdateRequestDto request);
    Task<ActionResult<AdminWorkspaceDto>> ChangeWorkspaceStatus(int workspaceId, [FromQuery] WorkspaceStatusType statusType);
    Task<ActionResult<AdminWorkspaceDto>> DeleteWorkspace(int workspaceId);

    Task<ActionResult<AdminWorkspacePricingsResponseDto>> GetWorkspacePricings(int workspaceId);
    Task<ActionResult<AdminWorkspaceStatusHistoryResponseDto>> GetWorkspaceStatuses(int workspaceId);
}


[AdminApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/workspace")]
public class AdminWorkspaceApiController
    (
        IWorkspaceService workspaceService, 
        IMapper mapper
    ) 
    : Controller, IAdminWorkspaceApi
{
    [HttpGet]
    public async Task<ActionResult<AdminWorkspacesResponseDto>> GetWorkspaces([FromQuery] AdminWorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceService.GetWorkspacesForAdmin(request);

        var page = Pagination.Paginate(workspaces, out int total, request.PageNumber, request.PageSize);
        var workspaceDtos = mapper.Map<IEnumerable<AdminWorkspaceDto>>(page);

        return Ok(new AdminWorkspacesResponseDto() 
        {
            Workspaces = workspaceDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminWorkspaceDto>> GetWorkspace(int id)
    {
        try
        {
            var workspace = await workspaceService.GetWorkspaceById(id);
            var workspaceDto = mapper.Map<AdminWorkspaceDto>(workspace);
            return workspaceDto;
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AdminWorkspaceDto>> CreateWorkspace([FromBody] WorkspaceCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var workspace = await workspaceService.CreateWorkspace(request);
            var worksapceDto = mapper.Map<AdminWorkspaceDto>(workspace);
            return Ok(worksapceDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<AdminWorkspaceDto>> UpdateWorkspace([FromBody] WorkspaceUpdateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var workspace = await workspaceService.UpdateWorkspace(request);
            return Ok(mapper.Map<AdminWorkspaceDto>(workspace));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<AdminWorkspaceDto>> ChangeWorkspaceStatus(int id, [FromQuery] WorkspaceStatusType statusType)
    {
        try
        {
            if (await workspaceService.UpdateWorkspaceStatus(id, statusType))
            {
                return Ok(new { message = $"Workspace status changed to {statusType}." });
            }
            
            return BadRequest(new { message = "Unable to change workspace status." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id:int}")]
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


    [HttpGet("{id:int}/pricings")]
    public async Task<ActionResult<AdminWorkspacePricingsResponseDto>> GetWorkspacePricings(int workspaceId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:int}/statuses")]
    public async Task<ActionResult<AdminWorkspaceStatusHistoryResponseDto>> GetWorkspaceStatuses(int workspaceId)
    {
        throw new NotImplementedException();
    }
}