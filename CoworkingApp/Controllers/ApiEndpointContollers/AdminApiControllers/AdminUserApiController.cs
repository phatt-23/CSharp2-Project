using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

public interface IAdminUserApi
{ 
    Task<ActionResult<IEnumerable<UserDto>>> GetUsers(UserQueryRequestDto request);
    Task<ActionResult<UserDto>> ChangeUserRole(int userId, [FromQuery] UserRoleType role);
    Task<ActionResult<UserDto>> RemoveUser(int userId);
}

[ApiController]
[AdminApiController]
[Route("api/admin/user")]
public class AdminUserApiController
    (
        IUserService userService, 
        IMapper mapper
    ) 
    : Controller, IAdminUserApi
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] UserQueryRequestDto? request = null)
    {
        try
        {
            var users = await userService.GetUsers(request ?? new UserQueryRequestDto());
            var userDtos = mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult<UserDto>> ChangeUserRole(int id, [FromQuery] UserRoleType role)
    {
        try
        {
            var users = await userService.ChangeUserRole(id, role);
            var userDto = mapper.Map<UserDto>(users);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<UserDto>> RemoveUser(int id)
    {
        try
        {
            var user = await userService.RemoveUser(id);
            var userDto = mapper.Map<UserDto>(user);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}