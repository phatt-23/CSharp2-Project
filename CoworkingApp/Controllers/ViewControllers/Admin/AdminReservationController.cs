using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.MVC.Admin;


[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminReservationController : Controller
{
    // Route: GET /admin/reservations
    public IActionResult Index()
    {
        return View();
    }

    // Route: GET /admin/reservations/edit/{id}
    public IActionResult Edit(int id)
    {
        return View();
    }

    // Route: GET /admin/reservations/delete/{id}
    public IActionResult Delete(int id)
    {
        return View();
    }
}
