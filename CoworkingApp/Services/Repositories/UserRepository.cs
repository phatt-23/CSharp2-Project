using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<IEnumerable<User>> GetUsers(UserFilter filter);
    Task<User> UpdateUser(User user);
}

public class UserRepository
    (
        CoworkingDbContext context
    ) 
    : IUserRepository
{
    public Task<IEnumerable<User>> GetUsers(UserFilter filter)
    {
        var query = context.Users.ApplyFilter(filter);

        query = filter.CreatedAt.ApplyTo(query, x => x.CreatedAt);
        query = filter.RefreshTokenExpiry.ApplyTo(query, x => x.RefreshTokenExpiry);

        if (filter.IncludeReservations)
            query = query.Include(x => x.Reservations);

        if (filter.IncludeUserRole)
            query = query.Include(x => x.Role);
        
        return Task.FromResult<IEnumerable<User>>(query);
    }

    public async Task<User> UpdateUser(User user)
    {
        var u = context.Users.Update(user);
        await context.SaveChangesAsync();
        return u.Entity;
    }

    public async Task<User> AddUser(User user)
    {
        var u = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return u.Entity;
    }

    public Task<User> RemoveUser(int userId)
    {
        var user = context.Users.Single(x => x.UserId == userId);
        user.IsRemoved
        // throw new NotImplementedException("User removal is not implemented. Add column is_removed into the database.");
        return Task.FromResult<User>(user);
    }
}

public class UserFilter : FilterBase
{
    [CompareTo(nameof(User.UserId))]
    public int? Id { get; set; }

    [CompareTo(nameof(User.Email))]
    public string? Email { get; set; }

    [CompareTo(nameof(User.RoleId))]
    public int? RoleId { get; set; }

    [CompareTo(nameof(User.IsRemoved))]
    public bool IsRemoved { get; set; } = false;


    public RangeFilter<DateTime> CreatedAt { get; set; } = new();
    public NullableRangeFilter<DateTime> RefreshTokenExpiry { get; set; } = new();
    public bool IncludeReservations { get; set; } = false;
    public bool IncludeUserRole { get; set; } = false;
}