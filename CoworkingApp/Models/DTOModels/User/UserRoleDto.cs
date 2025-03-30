namespace CoworkingApp.Models.DTOModels.User;

public class UserRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}