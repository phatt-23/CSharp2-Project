using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetRoles([FromQuery] UserRoleType? type = null)
    {
        if (type != null)
        {
            var role = await userRoleRepository.GetUserRole(type.Value);
            var roleDto = mapper.Map<UserRoleDto>(role);
            List<UserRoleDto> dtos = [roleDto];
            return Ok(dtos);
        }

        var roles = await userRoleRepository.GetUserRoles();
        var roleDtos = mapper.Map<IEnumerable<UserRoleDto>>(roles);
        return Ok(roleDtos);
    }
}
