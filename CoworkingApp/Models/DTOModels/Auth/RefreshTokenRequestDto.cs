namespace CoworkingApp.Models.DTOModels.Auth;

public class RefreshTokenRequestDto
{
    public int UserId { get; set; }
    public required string RefreshToken { get; set; }
}