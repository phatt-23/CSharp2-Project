using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IUserRepository
{
    Task<User>              AddUser(User user);
    Task<IEnumerable<User>> GetUsers(UserFilter filter);
    Task<User>              UpdateUser(User user);
    Task<User>              RemoveUser(User user);
}

public class UserRepository
    (
        CoworkingDbContext context
    ) 
    : IUserRepository
{
    public async Task<IEnumerable<User>> GetUsers(UserFilter filter)
    {
        var query = context.Users.ApplyFilter(filter);

        query = filter.CreatedAt.ApplyTo(query, x => x.CreatedAt);
        query = filter.RefreshTokenExpiry.ApplyTo(query, x => x.RefreshTokenExpiry);

        if (filter.IncludeReservations)
            query = query.Include(x => x.Reservations);

        if (filter.IncludeUserRole)
            query = query.Include(x => x.Role);
        
        return query;
    }

    public async Task<User> UpdateUser(User user)
    {
        var updatedUser = context.Users.Update(user);
        await context.SaveChangesAsync();
        return updatedUser.Entity;
    }

    public async Task<User> AddUser(User user)
    {
        var addedUser = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return addedUser.Entity;
    }

    public async Task<User> RemoveUser(User user)
    {
        var u = context.Users.Single(x => x.UserId == user.UserId);
        u.IsRemoved = true;

        var removedUser = context.Users.Update(u);
        await context.SaveChangesAsync();

        return removedUser.Entity;
    }
}

public class UserFilter : FilterBase
{
    [CompareTo(nameof(User.UserId))]
    public int? UserId { get; set; }


    [CompareTo(nameof(User.Email))]
    public string? Email { get; set; }

    [CompareTo(nameof(User.RoleId))]
    public int? RoleId { get; set; }

    [CompareTo(nameof(User.IsRemoved))]
    public bool? IsRemoved { get; set; }
    public RangeFilter<DateTime> CreatedAt { get; set; } = new();
    public NullableRangeFilter<DateTime> RefreshTokenExpiry { get; set; } = new();
    public bool IncludeReservations { get; set; } = false;
    public bool IncludeUserRole { get; set; } = false;
}
