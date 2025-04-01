using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

public interface IApiDocsApi
{
    Task<IActionResult> GetJsonDocsAsync([FromQuery] string? url);
}

[ApiController]
[Route("/api/docs")]
public class ApiDocsApiController : Controller, IApiDocsApi
{
    [HttpGet("json")]
    public async Task<IActionResult> GetJsonDocsAsync([FromQuery] string? url)
    {
        if (url == null)
        {
            return Ok(new { Message = "Json specification of every API endpoint this application offers" });
        }
        else
        {
            return Ok(new { Message = $"Json specification of API endpoint: {url}" });
        }
    }
}