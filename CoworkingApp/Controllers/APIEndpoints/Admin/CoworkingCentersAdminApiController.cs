using AutoMapper;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;


[ApiController]
[Route("/api/admin/coworking-centers")]
public class CoworkingCentersAdminApiController(
    CoworkingCentersService coworkingCentersService
    ) : Controller
{
    /// ADMIN - Create a new coworking center.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CoworkingCenterDto>> CreateAsync(CoworkingCenterCreateRequestDto request)
    {
        try
        {
            var coworkingCenterDto = await coworkingCentersService.CreateAsync(request);
            return coworkingCenterDto;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}
