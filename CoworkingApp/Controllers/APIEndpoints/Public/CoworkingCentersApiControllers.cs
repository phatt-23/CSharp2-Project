using AutoMapper;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

[ApiController]
[Route("/api/coworking-centers")]
public class CoworkingCentersApiController(
    ICoworkingCenterService coworkingCenterService,
    IPaginationService paginationService,
    IMapper mapper
    ) : Controller
{
    /// PUBLIC - Get filtered coworking centers.
    [HttpGet]
    public async Task<ActionResult<ICollection<CoworkingCenterDto>>> Get([FromQuery] CoworkingCenterQueryRequestDto request)
    {
        var centers = await coworkingCenterService.GetCoworkingCentersAsync(request);

        var totalCount = centers.Count();
        
        var paginatedCenters = paginationService.Paginate(centers, request.PageNumber, request.PageSize);
        var centerDtos = mapper.Map<IEnumerable<CoworkingCenterDto>>(paginatedCenters);
        
        var response = new CoworkingCentersResponseDto
        {
            Centers = centerDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
        
        return Ok(response);
    }

    /// PUBLIC - Get a coworking center by id.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CoworkingCenterDto>> GetById(int id)
    {
        try
        {
            var center = await coworkingCenterService.GetCoworkingCenterByIdAsync(id);
            
            var response = new CoworkingCenterDto
            {
                Name = center.Name,
                Description = center.Description,
                Latitude = center.Latitude,
                Longitude = center.Longitude
            };
            
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}