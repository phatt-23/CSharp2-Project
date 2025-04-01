using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

public interface IAdminUserApi
{ 
    // | `GET` | `/api/admin/users` | Get a list of all users. |
    // | `PUT` | `/api/admin/users/{id}/role` | Change a user's role (admin/user). |
    // | `DELETE` | `/api/admin/users/{id}` | Delete a user. |
}

[ApiController]
[Route("api/admin/user")]
public class AdminUserApiController : Controller, IAdminUserApi
{
    
}