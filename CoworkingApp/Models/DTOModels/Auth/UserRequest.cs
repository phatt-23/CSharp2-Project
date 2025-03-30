namespace CoworkingApp.Models.DTOModels.Auth;


public class UserRegisterRequestDto
{ 
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}


public class UserLoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}



