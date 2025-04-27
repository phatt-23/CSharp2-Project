using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IUserRoleRepository
{
    Task<UserRole> GetUserRole(UserRoleType roleType);
}

public class UserRoleRepository
    (
        CoworkingDbContext context
    ) 
    : IUserRoleRepository
{
    public async Task<UserRole> GetUserRole(UserRoleType roleType)
    {
        var role = await context.UserRoles.SingleAsync(x => x.Name == roleType.ToString());
        return role;
    }
}
