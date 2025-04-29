using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.AdminApiControllers;

public interface IAdminReservationApi
{
    Task<ActionResult<IEnumerable<AdminReservationDto>>> GetReservations([FromQuery] AdminReservationQueryRequestDto request);
    Task<ActionResult<AdminReservationDto>> GetReservationById(int id);
    Task<ActionResult<AdminReservationDto>> UpdateReservation(int id, [FromBody] AdminReservationUpdateRequestDto request);
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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminReservationDto>>> GetReservations([FromQuery] AdminReservationQueryRequestDto request)
    {
        try
        {
            var reservations = await reservationService.AdminGetReservations(request);
            var reservationDtos = mapper.Map<IEnumerable<AdminReservationDto>>(reservations);
            return Ok(reservationDtos);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AdminReservationDto>> UpdateReservation(int id, [FromBody] AdminReservationUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var reservation = await reservationService.AdminUpdateReservation(id, request);
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
