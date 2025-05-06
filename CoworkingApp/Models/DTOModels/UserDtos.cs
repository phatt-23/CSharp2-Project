using CoworkingApp.Models.DataModels;
using CoworkingApp.Services;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

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
    public UserRoleType Role { get; set; }
}

[PublicDataDto]
public class UserRoleDto
{
    public int UserRoleId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
}

[AdminDataDto]
public class AdminUserDto : UserDto
{
    public bool IsRemoved { get; set; }
}

//////////////////////////////////////////////////////
// Request DTOs
//////////////////////////////////////////////////////

[AdminRequestDto]
public class AdminUserQueryRequestDto : PaginationRequestDto
{
    public int? Id { get; set; }
    public string? Email { get; set; }
    public int? RoleId { get; set; }
    public RangeFilter<DateTime> CreatedAt { get; set; } = new();
    public RangeFilter<DateTime> RefreshTokenExpiry { get; set; } = new();
    public UserRoleType? Role { get; set; }
    public bool IncludeReservations { get; set; } = false;
}

[AdminRequestDto]
public class AdminUserRoleChangeRequestDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public UserRoleType Role { get; set; }
}

[AdminRequestDto]
public class AdminUserCreateDto
{
    [Required]
    [RegularExpression("@^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public UserRoleType Role { get; set; }
}

//////////////////////////////////////
/// Response DTO
//////////////////////////////////////

[AdminResponseDto]
public class AdminUsersResponseDto : PaginationResponseDto
{
    public required List<AdminUserDto> Users { get; set; }
}