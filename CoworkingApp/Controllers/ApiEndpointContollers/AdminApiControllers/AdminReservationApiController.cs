using AutoMapper;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

public interface IAdminReservationApi
{
    Task<ActionResult<IEnumerable<AdminReservationDto>>> GetReservationsAsync([FromQuery] AdminReservationQueryRequestDto request);
    Task<ActionResult<AdminReservationDto>> GetReservationByIdAsync(int id);
    Task<ActionResult<AdminReservationDto>> UpdateReservationAsync(int id, [FromBody] ReservationUpdateRequestDto request);
    Task<ActionResult<AdminReservationDto>> CancelReservationAsync(int id);
}

// | `GET` | `/api/admin/reservations` | Get all reservations (for management). |
// | `PUT` | `/api/admin/reservations/{id}` | Modify a reservation (e.g., change timing). | 
// | `DELETE` | `/api/admin/reservations/{id}` | Force cancel a reservation. |

[ApiController]
[Route("api/admin/reservation")]
public class AdminReservationApiController(
    IReservationService reservationService,
    IMapper mapper
    ) : Controller, IAdminReservationApi
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<AdminReservationDto>>> GetReservationsAsync([FromQuery] AdminReservationQueryRequestDto request)
    {
        try
        {
            var reservations = await reservationService.GetReservationsForAdminAsync(request);
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
    public async Task<ActionResult<AdminReservationDto>> GetReservationByIdAsync(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationByIdAsync(id);
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
    public async Task<ActionResult<AdminReservationDto>> UpdateReservationAsync(int id, [FromBody] ReservationUpdateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var reservation = await reservationService.UpdateReservationAsync(id, request);
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
    public async Task<ActionResult<AdminReservationDto>> CancelReservationAsync(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var updatedReservation = await reservationService
                .UpdateReservationAsync(id, new ReservationUpdateRequestDto { IsCancelled = true });

            var updatedReservationDto = mapper.Map<AdminReservationDto>(updatedReservation);
            return Ok(updatedReservationDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}