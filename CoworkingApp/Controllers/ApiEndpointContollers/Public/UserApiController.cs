using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

[PublicApiController]
[Route("/api/user")]
public class UserApiController
    (
        IUserRoleRepository userRoleRepository,
        IMapper mapper
    ) 
    : Controller
{
    [HttpGet("role")]
    public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetRoles()
    {
        var roles = await userRoleRepository.GetUserRoles();
        var roleDtos = mapper.Map<IEnumerable<UserRoleDto>>(roles);
        return Ok(roleDtos);
    }
}
