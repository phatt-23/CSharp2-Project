namespace CoworkingApp.Models.DTOModels.User;

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
