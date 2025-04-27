using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

public interface IAdminCoworkingCentersApi
{
    Task<ActionResult<IEnumerable<AdminCoworkingCenterDto>>> Get([FromQuery] CoworkingCenterQueryRequestDto? request = null);
    Task<ActionResult<AdminCoworkingCenterDto>> Create([FromBody] CoworkingCenterCreateRequestDto request);
    Task<ActionResult<AdminCoworkingCenterDto>> Update(int coworkingCenterId, [FromBody] CoworkingCenterUpdateRequestDto request);
}

[ApiController]
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<AdminCoworkingCenterDto>>> Get([FromQuery] CoworkingCenterQueryRequestDto? request = null)
    {
        try
        {
            var centers = await coworkingCenterService.GetCenters(request ?? new ());
            return Ok(centers);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminCoworkingCenterDto>> Create([FromBody] CoworkingCenterCreateRequestDto request)
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
    public async Task<ActionResult<AdminCoworkingCenterDto>> Update(int coworkingCenterId, [FromBody] CoworkingCenterUpdateRequestDto request)
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
