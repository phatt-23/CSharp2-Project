using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.MVC.Admin;


[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminPricingController : Controller
{
    // Route: GET /admin/pricing
    public IActionResult Index()
    {
        return View();
    }

    // Route: GET /admin/pricing/create/{workspaceId}
    public IActionResult Create(int workspaceId)
    {
        return View();
    }
}