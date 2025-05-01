using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

internal interface IWorkspaceApi
{
    Task<ActionResult<IEnumerable<WorkspaceDto>>> Get([FromQuery] WorkspaceQueryRequestDto request);
    Task<ActionResult<WorkspaceDto?>> Get(int id);
    Task<ActionResult<WorkspaceHistoriesResponseDto>> GetHistory(int id);
}


[PublicApiController]
[Route("/api/workspace")]
public class WorkspaceApiController
    (
        IWorkspaceService workspacesService,
        IReservationRepository reservationRepository,
        IMapper mapper
    ) 
    : Controller, IWorkspaceApi
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkspaceDto>>> Get([FromQuery] WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspacesService.GetWorkspaces(request);
        
        var workspacePage = Pagination.Paginate(workspaces, out int totalCount, request.PageNumber, request.PageSize);
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkspaceDto?>> Get(int id)
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

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<WorkspaceHistoriesResponseDto>> GetHistory(int id)
    {
        try
        {
            var workspace = await workspacesService.GetWorkspaceById(id);
            var histories = await workspacesService.GetWorkspaceHistory(id);

            var workspaceDto = mapper.Map<WorkspaceDto>(workspace);
            var historyDtos = mapper.Map<IEnumerable<WorkspaceHistoryDto>>(histories);

            return Ok(new WorkspaceHistoriesResponseDto
            {
                Workspace = workspaceDto,
                Histories = historyDtos
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id:int}/reservation")]
    public async Task<ActionResult<WorkspaceReservationsResponseDto>> GetReservations(int id)
    {
        try
        {
            var workspace = await workspacesService.GetWorkspaceById(id);
            var reservations = await reservationRepository.GetReservations(new ReservationsFilter
            {
                WorkspaceId = workspace.WorkspaceId,
                IsCancelled = false,
            });

            var workspaceDto = mapper.Map<WorkspaceDto>(workspace);
            var reservationDtos = mapper.Map<IEnumerable<AnonymousReservationDto>>(reservations);

            return Ok(new WorkspaceReservationsResponseDto
            {
                Reservations = [..reservationDtos],
                Workspace = workspaceDto,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
