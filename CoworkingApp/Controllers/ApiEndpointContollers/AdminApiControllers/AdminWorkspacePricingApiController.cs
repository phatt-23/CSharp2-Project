using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;





public interface IAdminWorkspacePricingApi
{
// | `GET` | `/api/admin/pricing` | Get all pricing rules for workspaces. |
// | `POST` | `/api/admin/pricing` | Set or update pricing for a workspace. |
    
}

public class AdminWorkspacePricingApiController : Controller, IAdminWorkspacePricingApi
{
    
}