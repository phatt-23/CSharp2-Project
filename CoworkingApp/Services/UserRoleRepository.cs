using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Services;

public interface IUserRoleRepository
{
    Task<UserRole> GetUserRoleAsync(UserRoleType roleType);
}

public class UserRoleRepository(CoworkingDbContext context) : IUserRoleRepository
{
    public async Task<UserRole> GetUserRoleAsync(UserRoleType roleType)
    {
        var role = context.UserRoles.Single(x => x.Name == roleType.ToString());
        return role;
    }

}