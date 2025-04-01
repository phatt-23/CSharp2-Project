namespace CoworkingApp.Models.DTOModels.Auth;

/////////////////////////////////////////////////
// Token
/////////////////////////////////////////////////

public class TokenResponseDto
{
    public required string AccessToken { get; set; } 
    public required string RefreshToken { get; set; } 
}


public class RefreshTokenRequestDto
{
    public required string RefreshToken { get; set; }
}

/////////////////////////////////////////////////
// User Login/Register
/////////////////////////////////////////////////

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



