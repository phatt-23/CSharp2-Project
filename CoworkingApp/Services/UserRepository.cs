using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.User;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync(UserFilter filter);
    Task<User> UpdateUserAsync(User user);
    Task<User> AddUserAsync(User user);
    Task<User> RemoveUserAsync(int userId);
}

public class UserRepository(CoworkingDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetUsersAsync(UserFilter filter)
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

    public async Task<User> UpdateUserAsync(User user)
    {
        var u = context.Users.Update(user);
        await context.SaveChangesAsync();
        return u.Entity;
    }

    public async Task<User> AddUserAsync(User user)
    {
        var u = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return u.Entity;
    }

    public async Task<User> RemoveUserAsync(int userId)
    {
        var user = context.Users.Single(x => x.Id == userId);
        // throw new NotImplementedException("User removal is not implemented. Add column is_removed into the database.");
        return user;
    }
}

public class UserFilter : FilterBase
{
    [CompareTo(nameof(User.Id))]
    public int? Id { get; set; }

    [CompareTo(nameof(User.Email))]
    public string? Email { get; set; }

    [CompareTo(nameof(User.RoleId))]
    public int? RoleId { get; set; }
    
    public RangeFilter<DateTime> CreatedAt { get; set; }
    public NullableRangeFilter<DateTime> RefreshTokenExpiry { get; set; }
    public bool IncludeReservations { get; set; } = false;
    public bool IncludeUserRole { get; set; } = false;
}