namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////////////////
// User login/register (request)
/////////////////////////////////////////////////

public class UserRegisterRequestDto
{ 
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class UserLoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

/////////////////////////////////////////////////
// Token (request & response)
/////////////////////////////////////////////////

public class RefreshTokenRequestDto
{
    public required string RefreshToken { get; set; }
}

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}




