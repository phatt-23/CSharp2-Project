using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.PublicApiControllers;

public interface ICoworkingCentersApi
{
    // GET /api/coworking-center
    Task<ActionResult<CoworkingCenterQueryResponseDto>> GetCenters([FromQuery] CoworkingCenterQueryRequestDto request);

    // GET /api/coworking-center/{id}
    Task<ActionResult<CoworkingCenterDto>> GetCenterById(int id);
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
    public async Task<ActionResult<CoworkingCenterQueryResponseDto>> GetCenters([FromQuery] CoworkingCenterQueryRequestDto request)
    {
        var centers = await coworkingCenterService.GetCenters(request);

        var totalCount = centers.Count();
        
        var paginatedCenters = Pagination.Paginate(centers, request.PageNumber, request.PageSize);
        var centerDtos = mapper.Map<IEnumerable<CoworkingCenterDto>>(paginatedCenters);
        
        var response = new CoworkingCenterQueryResponseDto
        {
            Centers = centerDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CoworkingCenterDto>> GetCenterById(int id)
    {
        try
        {
            var center = await coworkingCenterService.GetCenterById(id);
            return Ok(mapper.Map<CoworkingCenterDto>(center));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

