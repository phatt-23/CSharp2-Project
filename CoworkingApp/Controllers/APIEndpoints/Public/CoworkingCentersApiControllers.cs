using AutoMapper;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

[ApiController]
[Route("/api/coworking-centers")]
public class CoworkingCentersApiController(
    CoworkingCentersService coworkingCentersService
    ) : Controller
{
    /// PUBLIC - Get filtered coworking centers.
    [HttpGet]
    public async Task<ActionResult<ICollection<CoworkingCenterDto>>> Get([FromQuery] CoworkingCentersQueryDto query)
    {
        var (centerDtos, totalCount) = await coworkingCentersService.GetAsync(query);

        var response = new CoworkingCentersResponseDto
        {
            Centers = centerDtos,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
        };
        
        return Ok(response);
    }

    /// PUBLIC - Get a coworking center by id.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CoworkingCenterDto?>> GetById(int id)
    {
        var center = await coworkingCentersService.GetByIdAsync(id);
        if (center == null)
            return NotFound($"Center with id '{id}' not found");

        var response = new CoworkingCenterDto
        {
            Name = center.Name,
            Description = center.Description,
            Latitude = center.Latitude,
            Longitude = center.Longitude
        };
        
        return Ok(response);
    }
}