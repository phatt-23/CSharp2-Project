using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.User;

namespace CoworkingApp.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync(UserQueryRequestDto request);
    Task<User> ChangeUserRoleAsync(int userId, UserRoleType roleType);
    Task<User> RemoveUserAsync(int userId);
}
    
public class UserService(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository
    ) : IUserService
{
    public async Task<IEnumerable<User>> GetUsersAsync(UserQueryRequestDto request)
    {
        var users = await userRepository.GetUsersAsync(new UserFilter()
        {
            Id = request.Id,
            Email = request.Email,
            RoleId = request.RoleId,
            CreatedAt = request.CreatedAt,
            RefreshTokenExpiry = new NullableRangeFilter<DateTime>
                { Min = request.RefreshTokenExpiry.Min, Max = request.RefreshTokenExpiry.Max },
            IncludeUserRole = true,
            IncludeReservations = request.IncludeReservations,
        });

        return users;
    }

    public async Task<User> ChangeUserRoleAsync(int userId, UserRoleType roleType)
    {
        var users = await GetUsersAsync(new UserQueryRequestDto() { Id = userId });
        var user = users.Single();
        var role = await userRoleRepository.GetUserRoleAsync(roleType);

        user.RoleId = role.Id;

        return await userRepository.UpdateUserAsync(user);
    }

    public async Task<User> RemoveUserAsync(int userId)
    {
        return await userRepository.RemoveUserAsync(userId);
    }
}