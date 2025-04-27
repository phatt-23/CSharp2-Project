using Microsoft.AspNetCore.Mvc;


[Route("developer")]
public class DeveloperController : Controller 
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}