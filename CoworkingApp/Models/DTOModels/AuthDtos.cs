using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////////////////
// User login/register (request)
/////////////////////////////////////////////////

public class UserRegisterRequestDto
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string ConfirmPassword { get; set; } = null!;
}

public class UserLoginRequestDto
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
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




