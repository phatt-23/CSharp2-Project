using CoworkingApp.Models.DtoModels;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace CoworkingApp.Controllers.ApiEndpointContollers.PublicApiControllers;

public interface IApiDocsApi
{
    Task<IActionResult> GetJson([FromQuery] string? url);
    Task<IActionResult> GetAdminJson([FromQuery] string? url);
}

[ApiController]
[Route("/api/docs")]
public class ApiDocsApiController : Controller, IApiDocsApi
{
    [HttpGet]
    public async Task<IActionResult> 
        GetJson([FromQuery] string? url)
    {
        if (url != null)
        {
            return Ok(new { Message = $"Json specification of API endpoint: {url}" });
        }
        else
        {
            var endpoints = GetEndpoints();
            return Ok(endpoints);
        }
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> GetAdminJson([FromQuery] string? url)
    {
        if (url == null)
        {
            return Ok(new { Message = "Json specification of every ADMIN API endpoint" });
        }
        else
        {
            return Ok(new { Message = $"Json specification of this ADMIN API endpoint: {url}" });
        }
    }


    private static bool IsComplex(Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(decimal) || type == typeof(Guid))
            return false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return IsComplex(Nullable.GetUnderlyingType(type)!);

        return true;
    }

    private static string UnwrapReturnType(Type returnType)
    {
        if (returnType.IsGenericType && typeof(Task).IsAssignableFrom(returnType))
            returnType = returnType.GetGenericArguments().FirstOrDefault() ?? returnType;

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
            returnType = returnType.GetGenericArguments().FirstOrDefault() ?? returnType;

        return returnType.Name;
    }

    private static List<ApiFieldDescription> GetFieldsRecursive(Type type, HashSet<Type> visited)
    {
        if (visited.Contains(type)) // prevent infinite loops in case of cycles
            return new List<ApiFieldDescription>();

        visited.Add(type);

        var fields = new List<ApiFieldDescription>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var fieldType = prop.PropertyType;
            var isComplex = IsComplex(fieldType);

            var field = new ApiFieldDescription
            {
                Name = prop.Name,
                Type = fieldType.Name,
                IsComplexType = isComplex,
                SubFields = isComplex ? GetFieldsRecursive(fieldType, visited) : new List<ApiFieldDescription>()
            };

            fields.Add(field);
        }

        return fields;
    }

    private static List<ApiEndpointDescription> GetEndpoints()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var controllers = assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && t.GetCustomAttribute<ApiControllerAttribute>() != null);

        var publicControllers = controllers.Where(t => t.GetCustomAttribute<AdminApiControllerAttribute>() == null);
        var adminControllers = controllers.Where(t => t.GetCustomAttribute<AdminApiControllerAttribute>() != null);

        var endpoints = new List<ApiEndpointDescription>();

        foreach (var controller in controllers)
        {
            var controllerRoute = controller.GetCustomAttribute<RouteAttribute>()?.Template ?? "";

            var controllerRequiresAuth = controller.GetCustomAttribute<AuthorizeAttribute>() != null;
            var controllerAllowsAnonymous = controller.GetCustomAttribute<AllowAnonymousAttribute>() != null;

            var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                var httpMethodAttr = method.GetCustomAttributes()
                    .FirstOrDefault(attr => attr is HttpMethodAttribute) as HttpMethodAttribute;

                if (httpMethodAttr == null)
                    continue;

                var methodRoute = httpMethodAttr.Template ?? "";
                var fullRoute = controllerRoute.Replace("[controller]", controller.Name.Replace("Controller", "")).Trim('/') +
                                (string.IsNullOrWhiteSpace(methodRoute) ? "" : "/" + methodRoute.Trim('/'));

                var methodRequiresAuth = method.GetCustomAttribute<AuthorizeAttribute>() != null || controllerRequiresAuth;
                var methodAllowsAnonymous = method.GetCustomAttribute<AllowAnonymousAttribute>() != null || controllerAllowsAnonymous;

                var endpoint = new ApiEndpointDescription
                {
                    HttpMethod = httpMethodAttr.HttpMethods.FirstOrDefault() ?? "UNKNOWN",
                    Route = "/" + fullRoute.Trim('/'),
                    RequiresAuthentication = methodRequiresAuth,
                    AllowsAnonymous = methodAllowsAnonymous,
                    ReturnType = UnwrapReturnType(method.ReturnType),
                    Parameters = method.GetParameters()
                        .Select(p => new ApiParameterDescription
                        {
                            Name = p.Name,
                            Type = p.ParameterType.Name,
                            IsComplexType = IsComplex(p.ParameterType),
                            Fields = IsComplex(p.ParameterType) ? GetFieldsRecursive(p.ParameterType, new HashSet<Type>()) : new List<ApiFieldDescription>()
                        }).ToList()
                };

                endpoints.Add(endpoint);
            }
        }

        return endpoints;
    }
}