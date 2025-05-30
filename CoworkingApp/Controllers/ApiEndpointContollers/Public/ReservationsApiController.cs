using System.Security.Claims;
using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Models.Misc;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

internal interface IReservationsApi
{
    Task<ActionResult<ReservationsResponseDto>> GetResevations([FromQuery] ReservationQueryRequestDto request);
    Task<ActionResult<ReservationDto>> GetReservationById(int id);
    Task<IActionResult> CreateReservation([FromBody] ReservationCreateRequestDto request);
    Task<IActionResult> CancelReservation(int id);
}

[PublicApiController]
[Route("api/reservation")]
[Authorize]
public class ReservationsApiController
    (
        IReservationService reservationService,
        IMapper mapper
    ) 
    : Controller, IReservationsApi
{
    [HttpGet]
    public async Task<ActionResult<ReservationsResponseDto>> GetResevations([FromQuery] ReservationQueryRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.GetUserId()!;
        
        var reservations = (await reservationService.GetReservations(request))
            .Where(x => x.IsCancelled == false && x.CustomerId == userId);
        
        var paginatedReservations = Pagination.Paginate(reservations, out int totalCount, request.PageNumber, request.PageSize);
        var reservationDtos = mapper.Map<IEnumerable<ReservationDto>>(paginatedReservations);
           
        return Ok(new ReservationsResponseDto
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Reservations = reservationDtos,
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationDto>> GetReservationById(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);

            if (reservation.IsCancelled)
            {
                return BadRequest("Reservation is cancelled.");
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            
            if (reservation.CustomerId != int.Parse(userId))
            {
                return Unauthorized("Cannot access reservation not belonging to your account.");
            }
            
            var reservationDto = mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateRequestDto request)
    {
        try
        {
            var customerId = User.GetUserId()!;
            var reservation = await reservationService.CreateReservation(customerId.Value, request);
            var reservationDto = mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);
            
            if (reservation.IsCancelled)
            {
                return BadRequest("Reservation is already cancelled.");
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (userId != reservation.CustomerId.ToString())
            {
                return Unauthorized("Cannot cancel reservation not belonging to your account.");
            }

            var canceledReservation = await reservationService.CancelReservation(reservation.ReservationId);
            var reservationDto = mapper.Map<ReservationDto>(canceledReservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}