using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.AdminApiControllers;

public interface IAdminCoworkingCentersApi
{
    Task<ActionResult<AdminCoworkingCenterQueryResponseDto>> GetCenters([FromQuery] CoworkingCenterQueryRequestDto? request = null);
    Task<ActionResult<AdminCoworkingCenterDto>> GetCenter(int id);
    Task<ActionResult<AdminCoworkingCenterDto>> CreateCenter([FromBody] CoworkingCenterCreateRequestDto request);
    Task<ActionResult<AdminCoworkingCenterDto>> UpdateCenter(CoworkingCenterUpdateRequestDto request);
    Task<ActionResult<AdminCoworkingCenterDto>> DeleteCenter(int id);
}

[AdminApiController]
[Route("/api/admin/coworking-center")]
[Authorize(Roles = "Admin")]
public class AdminCoworkingCenterApiController
    (
        ICoworkingCenterService coworkingCenterService,
        IMapper mapper
    ) 
    : Controller, IAdminCoworkingCentersApi
{
    [HttpGet]
    public async Task<ActionResult<AdminCoworkingCenterQueryResponseDto>> GetCenters([FromQuery] CoworkingCenterQueryRequestDto request)
    {
        try
        {
            var centers = await coworkingCenterService.GetCenters(request);
            var page = Pagination.Paginate(centers, out int total, request.PageNumber, request.PageSize);

            return Ok(new AdminCoworkingCenterQueryResponseDto
            {
                Centers = mapper.Map<IEnumerable<AdminCoworkingCenterDto>>(page),
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminCoworkingCenterDto>> GetCenter(int id)
    {
        try
        {
            var center = await coworkingCenterService.GetCenterById(id);
            var centerDto = mapper.Map<AdminCoworkingCenterDto>(center);
            return centerDto;
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AdminCoworkingCenterDto>> CreateCenter([FromBody] CoworkingCenterCreateRequestDto request)
    {
        try
        {
            var coworkingCenter = await coworkingCenterService.CreateCenter(request);
            var coworkingCenterDto = mapper.Map<AdminCoworkingCenterDto>(coworkingCenter);
            return Ok(coworkingCenterDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<AdminCoworkingCenterDto>> UpdateCenter([FromBody] CoworkingCenterUpdateRequestDto request)
    {
        try
        {
            var coworkingCenter = await coworkingCenterService.UpdateCenter(request);
            return Ok(mapper.Map<AdminCoworkingCenterDto>(coworkingCenter));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<AdminCoworkingCenterDto>> DeleteCenter(int id)
    {
        try
        {
            var coworkingCenter = await coworkingCenterService.DeleteCenter(id);
            return Ok(mapper.Map<AdminCoworkingCenterDto>(coworkingCenter));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
