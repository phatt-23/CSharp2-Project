using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

public interface IAdminReservationApi
{
    Task<ActionResult<IEnumerable<AdminReservationDto>>> Get([FromQuery] AdminReservationQueryRequestDto request);
    Task<ActionResult<AdminReservationDto>> GetById(int id);
    Task<ActionResult<AdminReservationDto>> Update(int id, [FromBody] ReservationUpdateRequestDto request);
    Task<ActionResult<AdminReservationDto>> Cancel(int id);
}

[ApiController]
[AdminApiController]
[Route("api/admin/reservation")]
public class AdminReservationApiController
    (
        IReservationService reservationService,
        IMapper mapper
    ) 
    : Controller, IAdminReservationApi
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<AdminReservationDto>>> Get([FromQuery] AdminReservationQueryRequestDto request)
    {
        try
        {
            var reservations = await reservationService.GetReservationsForAdmin(request);
            var reservationDtos = mapper.Map<IEnumerable<AdminReservationDto>>(reservations);
            return Ok(reservationDtos);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminReservationDto>> GetById(int id)
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminReservationDto>> Update(int id, [FromBody] ReservationUpdateRequestDto request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        try
        {
            var reservation = await reservationService.UpdateReservation(id, request);
            var dto = mapper.Map<AdminReservationDto>(reservation);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminReservationDto>> Cancel(int id)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        try
        {
            var updatedReservation = await reservationService.UpdateReservation(id, new ReservationUpdateRequestDto { IsCancelled = true });
            return Ok(mapper.Map<AdminReservationDto>(updatedReservation));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}