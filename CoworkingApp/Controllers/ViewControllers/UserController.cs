using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.MVC;

public interface IUserView
{
    Task<IActionResult> GetUserProfile();
    Task<IActionResult> GetUserReservations();
    Task<IActionResult> CreateReservation(int id);
    Task<IActionResult> CancelReservation(int id);
}

[Route("user")]
public class UserController : Controller, IUserView
{
    [Authorize]
    [HttpGet("profile")]
    public Task<IActionResult> GetUserProfile()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("reservations")]
    public Task<IActionResult> GetUserReservations()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("reservations/new")]
    public Task<IActionResult> CreateReservation(int id)
    {
        throw new NotImplementedException();
    }
    
    [Authorize]
    [HttpDelete("reservations/{id}/cancel")]
    public Task<IActionResult> CancelReservation(int id)
    {
        throw new NotImplementedException();
    }
}