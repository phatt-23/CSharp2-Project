using CoworkingApp.Models.DataModels;
using CoworkingApp.Services;

namespace CoworkingApp.Models.DtoModels;

//////////////////////////////////////////////////////
// Data DTOs
//////////////////////////////////////////////////////

[AdminDto] 
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    // public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRoleDto Role { get; set; } = null!;
}

[AdminDto]
public class UserRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

//////////////////////////////////////////////////////
// Request DTOs
//////////////////////////////////////////////////////

public class UserQueryRequestDto
{
    public int? Id { get; set; }
    public string? Email { get; set; }
    public int? RoleId { get; set; }
    public RangeFilter<DateTime> CreatedAt { get; set; } = new();
    public RangeFilter<DateTime> RefreshTokenExpiry { get; set; } = new();
    public UserRoleType? Role { get; set; }
    public bool IncludeReservations { get; set; } = false;
}


public class UserRoleChangeRequestDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
}