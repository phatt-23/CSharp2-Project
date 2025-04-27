using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

public interface ICoworkingCentersApi
{
    // GET /api/coworking-center
    Task<ActionResult<ICollection<CoworkingCenterDto>>> Get([FromQuery] CoworkingCenterQueryRequestDto request);
    // GET /api/coworking-center/{id}
    Task<ActionResult<CoworkingCenterDto>> GetById(int id);
}

[ApiController]
[PublicApiController]
[Route("/api/coworking-center")]
public class CoworkingCentersApiController
    (
        ICoworkingCenterService coworkingCenterService,
        IMapper mapper
    ) 
    : Controller, ICoworkingCentersApi
{
    [HttpGet]
    public async Task<ActionResult<ICollection<CoworkingCenterDto>>> Get([FromQuery] CoworkingCenterQueryRequestDto request)
    {
        var centers = await coworkingCenterService.GetCoworkingCentersAsync(request);

        var totalCount = centers.Count();
        
        var paginatedCenters = Pagination.Paginate(centers, request.PageNumber, request.PageSize);
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CoworkingCenterDto>> GetById(int id)
    {
        try
        {
            var center = await coworkingCenterService.GetCoworkingCenterByIdAsync(id);

            var centerDto = mapper.Map<CoworkingCenterDto>(center);
            
            return Ok(centerDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

