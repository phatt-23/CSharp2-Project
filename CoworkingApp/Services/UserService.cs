using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CoworkingApp.Services;

public interface IUserService
{
    Task<User> CreateUser(AdminUserCreateDto request);
    Task<IEnumerable<User>> GetUsers(AdminUserQueryRequestDto request);
    Task<User> GetUserById(int userId);
    Task<User> ChangeUserRole(int userId, UserRoleType roleType);
    Task<User> RemoveUser(int userId);
    Task<User> LoginUser(UserLoginRequestDto request);
}
    
public class UserService
    (
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IMapper mapper
    )
    : IUserService
{
    public async Task<User> CreateUser(AdminUserCreateDto request)
    {
        if ((await userRepository.GetUsers(new UserFilter { Email = request.Email })).Any())
        {
            throw new EmailTakenException("Email already exists");
        }

        //var user = mapper.Map<User>(request);
        var user = new User()
        {
            Email = request.Email,
        };

        var role = await userRoleRepository.GetUserRole(request.Role);
        user.RoleId = role.UserRoleId;

        var passwordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;

        return await userRepository.AddUser(user);
    }

    public async Task<IEnumerable<User>> GetUsers(AdminUserQueryRequestDto request)
    {
        return await userRepository.GetUsers(new UserFilter()
            {
                UserId = request.Id,
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

    public async Task<User> GetUserById(int userId)
    {
        var users = await GetUsers(new AdminUserQueryRequestDto() { Id = userId });
        return users.Single();
    }

    public async Task<User> ChangeUserRole(int userId, UserRoleType roleType)
    {
        var users = await GetUsers(new AdminUserQueryRequestDto() { Id = userId });
        var user = users.Single();
        var role = await userRoleRepository.GetUserRole(roleType);

        user.RoleId = role.UserRoleId;

        return await userRepository.UpdateUser(user);
    }

    public async Task<User> RemoveUser(int userId)
    {
        var users = await userRepository.GetUsers(new UserFilter() { UserId = userId });
        return await userRepository.RemoveUser(users.Single());
    }

    public async Task<User> LoginUser(UserLoginRequestDto request)
    {
        var user = (await userRepository.GetUsers(new UserFilter
        {
            Email = request.Email,
            IncludeUserRole = true,
        })).FirstOrDefault() ?? throw new NotFoundException("User with this email does not exist");

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            throw new WrongPasswordException("Password verification failed");
        }

        return user;
    }
}