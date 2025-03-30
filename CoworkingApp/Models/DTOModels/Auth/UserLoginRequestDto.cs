namespace CoworkingApp.Models.DTOModels.Auth;

public class UserLoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}