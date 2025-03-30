using AutoMapper;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

[ApiController]
[Route("api/reservations")]
public class ReservationsApiController(
    ReservationsService reservationsService
    ) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] ReservationsQueryDto query)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (query.PageNumber < 1 || query.PageSize < 1)
            return BadRequest("PageNumber and PageSize must be greater than or equal to 1");
        
        var (reservationDtos, totalCount) = await reservationsService.GetAsync(query);

        if (totalCount == 0)
            return NoContent();
        
        var response = new ReservationsResponseDto
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount, 
            Reservation = reservationDtos
        };
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var reservationDto = await reservationsService.GetByIdAsync(id);
            return Ok(reservationDto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ReservationCreateRequestDto request)
    {
        try
        {
            var reservationDto = await reservationsService.CreateAsync(request);
            return Ok(reservationDto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var reservationDto = await reservationsService.CancelAsync(id);
            return Ok(reservationDto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

}