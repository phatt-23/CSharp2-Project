using CoworkingApp.Models.DataModels;
using CoworkingApp.Services;

namespace CoworkingApp.Models.DtoModels;

//////////////////////////////////////////////////////
// Data DTOs
//////////////////////////////////////////////////////

[PublicDataDto] 
public class UserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
}

[PublicDataDto]
public class UserRoleDto
{
    public int UserRoleId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
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