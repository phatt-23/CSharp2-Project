using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsers(UserQueryRequestDto request);
    Task<User> ChangeUserRole(int userId, UserRoleType roleType);
    Task<User> RemoveUser(int userId);
}
    
public class UserService
    (
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository
    )
    : IUserService
{
    public async Task<IEnumerable<User>> GetUsers(UserQueryRequestDto request)
    {
        return await userRepository.GetUsers(new UserFilter()
            {
                Id = request.Id,
                Email = request.Email,
                RoleId = request.RoleId,
                CreatedAt = request.CreatedAt,
                RefreshTokenExpiry = new NullableRangeFilter<DateTime>
                {
                    Min = request.RefreshTokenExpiry.Min,
                    Max = request.RefreshTokenExpiry.Max
                },
                IncludeUserRole = true,
                IncludeReservations = request.IncludeReservations,
            });
    }

    public async Task<User> ChangeUserRole(int userId, UserRoleType roleType)
    {
        var users = await GetUsers(new UserQueryRequestDto() { Id = userId });
        var user = users.Single();
        var role = await userRoleRepository.GetUserRole(roleType);

        user.RoleId = role.UserRoleId;

        return await userRepository.UpdateUser(user);
    }

    public async Task<User> RemoveUser(int userId)
    {
        return await userRepository.RemoveUser(userId);
    }
}