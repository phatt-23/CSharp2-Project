using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Types;

[Authorize(Roles = "Admin")]
public class AdminApiControllerAttribute : ApiControllerAttribute
{
}
