using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

// | `GET` | `/api/admin/users` | Get a list of all users. |
// | `PUT` | `/api/admin/users/{id}/role` | Change a user's role (admin/user). |
// | `DELETE` | `/api/admin/users/{id}` | Delete a user. |

public interface IAdminUserApi
{ 
    Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync(UserQueryRequestDto request);
    Task<ActionResult<UserDto>> ChangeUserRoleAsync(int userId, [FromQuery] UserRoleType role);
    Task<ActionResult<UserDto>> RemoveUserAsync(int userId);
}

[ApiController]
[Route("api/admin/user")]
public class AdminUserApiController(IUserService userService, IMapper mapper) : Controller, IAdminUserApi
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync(UserQueryRequestDto request)
    {
        try
        {
            var users = await userService.GetUsersAsync(request);
            var userDtos = mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult<UserDto>> ChangeUserRoleAsync(int id, [FromQuery] UserRoleType role)
    {
        try
        {
            var users = await userService.ChangeUserRoleAsync(id, role);
            var userDto = mapper.Map<UserDto>(users);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<UserDto>> RemoveUserAsync(int id)
    {
        try
        {
            var user = await userService.RemoveUserAsync(id);
            var userDto = mapper.Map<UserDto>(user);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}