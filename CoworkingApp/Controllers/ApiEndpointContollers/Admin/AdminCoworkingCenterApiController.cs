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
    Task<ActionResult<AdminCoworkingCenterDto>> UpdateCenter(int coworkingCenterId, [FromBody] CoworkingCenterUpdateRequestDto request);
}

[AdminApiController]
[Route("/api/admin/coworking-center")]
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

            return Ok(new AdminCoworkingCenterQueryResponseDto
            {
                Centers = mapper.Map<IEnumerable<AdminCoworkingCenterDto>>(centers),
                TotalCount = centers.Count(),
                PageNumber = request?.PageNumber ?? 1,
                PageSize = request?.PageSize ?? 10
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminCoworkingCenterDto>> UpdateCenter(int coworkingCenterId, [FromBody] CoworkingCenterUpdateRequestDto request)
    {
        try
        {
            var coworkingCenter = await coworkingCenterService.UpdateCenter(coworkingCenterId, request);
            return Ok(mapper.Map<AdminCoworkingCenterDto>(coworkingCenter));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
