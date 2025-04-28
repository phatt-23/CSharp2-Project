namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////////////////
// User login/register (request)
/////////////////////////////////////////////////

public class UserRegisterRequestDto
{ 
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
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




