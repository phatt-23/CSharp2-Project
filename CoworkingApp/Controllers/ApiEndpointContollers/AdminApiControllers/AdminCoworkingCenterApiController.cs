using AutoMapper;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;


public interface IAdminCoworkingCentersApi
{
    
}


[ApiController]
[Route("/api/admin/coworking-centers")]
public class AdminCoworkingCenterApiController(
    ICoworkingCenterService coworkingCenterService,
    IMapper mapper
    ) : Controller, IAdminCoworkingCentersApi
{
    /// ADMIN - Create a new coworking center.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CoworkingCenterDto>> CreateAsync(CoworkingCenterCreateRequestDto request)
    {
        try
        {
            var coworkingCenter = await coworkingCenterService.CreateCoworkingCenterAsync(request);
            var coworkingCenterDto = mapper.Map<CoworkingCenterDto>(coworkingCenter);
            return Ok(coworkingCenterDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
