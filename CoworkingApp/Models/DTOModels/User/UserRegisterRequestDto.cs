namespace CoworkingApp.Models.DTOModels.User;

public class UserRegisterRequestDto
{ 
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}