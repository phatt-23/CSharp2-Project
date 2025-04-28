using System.Security.Claims;
using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.PublicApiControllers;

internal interface IReservationsApi
{
    Task<IActionResult> GetResevationsAsync([FromQuery] ReservationQueryRequestDto request);
    Task<IActionResult> GetReservationByIdAsync(int id);
    Task<IActionResult> CreateReservationAsync([FromBody] ReservationCreateRequestDto request);
    Task<IActionResult> CancelReservationAsync(int id);
}

[ApiController]
[Route("api/reservation")]
public class ReservationsApiController
    (
        IReservationService reservationService,
        IMapper mapper
    ) 
    : Controller, IReservationsApi
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetResevationsAsync([FromQuery] ReservationQueryRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        // TODO: Replace user id:int with uuid:string
        request.CustomerId = int.Parse(userId);
        
        var reservations = await reservationService.GetReservations(request);

        reservations = reservations.Where(x => x.IsCancelled == false);
        
        var totalCount = reservations.Count();
       
        var paginatedReservations = Pagination.Paginate(reservations, request.PageNumber, request.PageSize);
        var reservationDtos = mapper.Map<IEnumerable<ReservationDto>>(paginatedReservations);
           
        var response = new ReservationsResponseDto
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount, 
            Reservations = reservationDtos
        };
        
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReservationByIdAsync(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);

            if (reservation.IsCancelled)
                return BadRequest("Reservation is already cancelled.");
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            
            if (reservation.CustomerId != int.Parse(userId))
                return Unauthorized("Cannot access reservation not belonging to your account.");
            
            var reservationDto = mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationCreateRequestDto request)
    {
        try
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var reservation = await reservationService.CreateReservation(int.Parse(customerId), request);
            var reservationDto = mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelReservationAsync(int id)
    {
        try
        {
            var reservation = await reservationService.GetReservationById(id);
            
            if (reservation.IsCancelled)
                return BadRequest("Reservation is already cancelled.");
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (userId != reservation.CustomerId.ToString())
                return Unauthorized("Cannot cancel reservation not belonging to your account.");

            var canceledReservation = await reservationService.CancelReservation(reservation.ReservationId);
            var reservationDto = mapper.Map<ReservationDto>(canceledReservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}