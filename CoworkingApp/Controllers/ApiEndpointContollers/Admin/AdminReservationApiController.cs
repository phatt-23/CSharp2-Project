using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.AdminApiControllers;

public interface IAdminReservationApi
{
    Task<ActionResult<AdminReservationDto>> CreateReservation([FromBody] AdminReservationCreateDto request);
    Task<ActionResult<AdminReservationsResponseDto>> GetReservations([FromQuery] AdminReservationQueryRequestDto request);
    Task<ActionResult<AdminReservationDto>> GetReservationById(int id);
    Task<ActionResult<AdminReservationDto>> UpdateReservation([FromBody] AdminReservationUpdateRequestDto request);
    Task<ActionResult<AdminReservationDto>> CancelReservation(int id);
}

[AdminApiController]
[Route("api/admin/reservation")]
[Authorize(Roles = "Admin")]
public class AdminReservationApiController
    (
        IReservationService reservationService,
        IMapper mapper
    ) 
    : Controller, IAdminReservationApi
{
    [HttpPost]
    public async Task<ActionResult<AdminReservationDto>> CreateReservation([FromBody] AdminReservationCreateDto request)
    {
        try
        {
            var reservation = await reservationService.CreateReservation(request.CustomerId, request);
            return Ok(mapper.Map<AdminReservationDto>(reservation));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<AdminReservationsResponseDto>> GetReservations([FromQuery] AdminReservationQueryRequestDto request)
    {
        try
        {
            var reservations = await reservationService.AdminGetReservations(request);
            if (request.WorkspaceId != null)
            {
                reservations = reservations.Where(r => r.WorkspaceId == request.WorkspaceId);
            }

            var page = Pagination.Paginate(reservations, out int total, request.PageNumber, request.PageSize);
            var reservationDtos = mapper.Map<List<AdminReservationDto>>(page);

            return Ok(new AdminReservationsResponseDto() 
            {
                Reservations = reservationDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = total,
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminReservationDto>> GetReservationById(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);
            var reservationDto = mapper.Map<AdminReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<AdminReservationDto>> UpdateReservation([FromBody] AdminReservationUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var reservation = await reservationService.AdminUpdateReservation(request);
            var reservationDto = mapper.Map<AdminReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<AdminReservationDto>> CancelReservation(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var canceledReservation = await reservationService.CancelReservation(id);
            var reservationDto = mapper.Map<AdminReservationDto>(canceledReservation);
            return Ok(reservationDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
