using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.MVC.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminWorkspaceController : Controller
{
    // Route: GET /admin/workspace
    public IActionResult Index()
    {
        return View();
    }

    // Route: GET /admin/workspace/create
    public IActionResult Create()
    {
        return View();
    }

    // Route: GET /admin/workspace/edit/{id}
    public IActionResult Edit(int id)
    {
        return View();
    }

    // Route: GET /admin/workspace/{id}/status
    public IActionResult UpdateStatus(int id)
    {
        return View();
    }

    // Route: GET /admin/workspace/delete/{id}
    public IActionResult Delete(int id)
    {
        return View();
    }
}
