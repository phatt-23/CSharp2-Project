using AutoMapper;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

internal interface IReservationsApi
{
    Task<IActionResult> GetResevationsAsync([FromQuery] ReservationQueryRequestDto request);
    Task<IActionResult> GetReservationByIdAsync(int id);
    Task<IActionResult> CreateReservationAsync([FromBody] ReservationCreateRequestDto request);
    Task<IActionResult> CancelReservationAsync(int id);
}


[ApiController]
[Route("api/reservations")]
public class ReservationsApiController(
    IReservationsService reservationsService,
    IPaginationService paginationService,
    IMapper mapper
    ) : Controller, IReservationsApi
{
    [HttpGet]
    public async Task<IActionResult> GetResevationsAsync([FromQuery] ReservationQueryRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (request.PageNumber < 1 || request.PageSize < 1)
            return BadRequest("PageNumber and PageSize must be greater than or equal to 1");
        
        var reservations = await reservationsService.GetReservationsAsync(request);
        if (!reservations.Any())
            return NoContent();
        
        var totalCount = reservations.Count();
       
        var paginatedResevations = paginationService.Paginate(reservations, request.PageNumber, request.PageSize);
        var reservationDtos = mapper.Map<IEnumerable<ReservationDto>>(paginatedResevations);
           
        var response = new ReservationsResponseDto
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount, 
            Reservations = reservationDtos
        };
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReservationByIdAsync(int id)
    {
        try
        {
            var reservation = await reservationsService.GetReservationByIdAsync(id);
            var reservationDto = mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationCreateRequestDto request)
    {
        try
        {
            var reservation = await reservationsService.CreateReservationAsync(request);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelReservationAsync(int id)
    {
        try
        {
            var reservation = await reservationsService.CancelReservationAsync(id);
            var reservationDto = mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}