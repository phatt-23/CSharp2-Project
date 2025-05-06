using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.AdminApiControllers;

public interface IAdminUserApi
{
    Task<ActionResult<AdminUserDto>> CreateUser(AdminUserCreateDto request);
    Task<ActionResult<AdminUsersResponseDto>> GetUsers(AdminUserQueryRequestDto request);
    Task<ActionResult<AdminUserDto>> GetUser(int id);
    Task<ActionResult<AdminUserDto>> RemoveUser(int userId);
    Task<ActionResult<AdminUserDto>> ChangeUserRole(AdminUserRoleChangeRequestDto request);
}

[AdminApiController]
[Route("api/admin/user")]
[Authorize(Roles = "Admin")]
public class AdminUserApiController
    (
        IUserService userService, 
        IMapper mapper
    ) 
    : Controller, IAdminUserApi
{
    [HttpPost]
    public async Task<ActionResult<AdminUserDto>> CreateUser(AdminUserCreateDto request)
    {
        try 
        {
            var user = await userService.CreateUser(request);
            return Ok(mapper.Map<AdminUserDto>(user));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<AdminUsersResponseDto>> GetUsers([FromQuery] AdminUserQueryRequestDto request)
    {
        try
        {
            var users = await userService.GetUsers(request);
            var page = Pagination.Paginate(users, out int total, request.PageNumber, request.PageSize);
            var userDtos = mapper.Map<List<AdminUserDto>>(page);

            return Ok(new AdminUsersResponseDto()
            { 
                Users = userDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = total,
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminUserDto>> GetUser(int id)
    {
        try
        {
            var user = await userService.GetUserById(id);
            var userDto = mapper.Map<AdminUserDto>(user);
            return userDto;
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<AdminUserDto>> RemoveUser(int id)
    {
        try
        {
            var user = await userService.RemoveUser(id);
            var userDto = mapper.Map<AdminUserDto>(user);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("role")]
    public async Task<ActionResult<AdminUserDto>> ChangeUserRole([FromBody] AdminUserRoleChangeRequestDto request)
    {
        try
        {
            var users = await userService.ChangeUserRole(request.UserId, request.Role);
            var userDto = mapper.Map<AdminUserDto>(users);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}