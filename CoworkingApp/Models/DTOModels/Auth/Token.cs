namespace CoworkingApp.Models.DTOModels.Auth;


public class TokenResponseDto
{
    public required string AccessToken { get; set; } 
    public required string RefreshToken { get; set; } 
}


public class RefreshTokenRequestDto
{
    public required string RefreshToken { get; set; }
}