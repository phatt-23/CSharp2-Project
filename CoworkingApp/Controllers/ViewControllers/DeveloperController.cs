using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ViewControllers;

[Route("developer")]
public class DeveloperController : Controller 
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}