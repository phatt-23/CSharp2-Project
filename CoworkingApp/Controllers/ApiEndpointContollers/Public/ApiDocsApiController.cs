using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.OpenApi.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography.Xml;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

[PublicApiController]
[Route("/api/docs")]
public class ApiDocsApiController : Controller
{
    [HttpGet]
    public async Task<ActionResult<ApiEndpointsResponseDto>> GetEndpointsJson()
    {
        return Ok(Foo());
    }

    private static ApiEndpointsResponseDto Foo()
    {
        var endpoints = GetEndpoints();
        return new ApiEndpointsResponseDto()
        {
            Endpoints = endpoints,
            Schemas = Schemas,
        };
    }

    private static Dictionary<string, PathItem> GetEndpoints()
    {
        var endpoints = new List<object>();

        var assembly = Assembly.GetExecutingAssembly();
        var apiControllers = assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Controller)) && t.GetCustomAttribute<ApiControllerAttribute>() != null);

        var paths = new Dictionary<string, PathItem>();

        foreach (var apiController in apiControllers)
        {
            var controllerPaths = GetPathItems(apiController);
            foreach (var (key, value) in controllerPaths)
                paths.Add(key, value);
        }

        return paths;
    }

    private static Dictionary<string, PathItem> GetPathItems(Type controller)
    {
        var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
        var controllerName = controller.Name.Replace("Controller", "");
        var controllerRoute = controller.GetCustomAttribute<RouteAttribute>()?.Template ?? controllerName;
        var authorizeAttribute = controller.GetCustomAttribute<AuthorizeAttribute>();
        var controllerRequiresAuth = authorizeAttribute != null;
        var controllerRequiresAuthorization = !string.IsNullOrEmpty(authorizeAttribute?.Roles);

        Dictionary<string, PathItem> pathItems = [];

        foreach (var method in methods)
        {
            var httpAttr = method.GetCustomAttribute<HttpMethodAttribute>()!;
            var httpMethod = httpAttr.HttpMethods.FirstOrDefault() ?? "UNKNOWN";
            var methodRoute = (httpAttr?.Template == null) 
                ? "" 
                : (controllerRoute.EndsWith("/") ? "" : "/") + httpAttr?.Template;

            var op = CreateOperation(method);

            if (controllerRequiresAuth)
                op.RequiresAuthentication = controllerRequiresAuth;

            if (controllerRequiresAuthorization && op.Authorities.Count == 0)
                op.Authorities = authorizeAttribute?.Roles?.Split(',').ToList() ?? [];

            var path = controllerRoute + methodRoute;

            if (!pathItems.ContainsKey(path)) 
                pathItems[path] = new();

            switch (httpMethod)
            {
                case "GET":
                    pathItems[path].Get = op;
                    break;
                case "POST":
                    pathItems[path].Post = op;
                    break;
                case "PUT":
                    pathItems[path].Put = op;
                    break;
                case "DELETE":
                    pathItems[path].Delete = op;
                    break;
                default:
                    throw new NotSupportedException($"HTTP method {httpMethod} is not supported.");
            }
        }

        return pathItems;
    }

    private static Operation CreateOperation(MethodInfo method)
    {
        var authorizeAttribute = method.GetCustomAttribute<AuthorizeAttribute>();
        var parameters = GetParameters(method);
        var response = GetResponse(method);

        return new Operation()
        {
            Parameters = parameters,
            Response = response,
            RequiresAuthentication = authorizeAttribute != null,
            Authorities = authorizeAttribute?.Roles?.Split(',').ToList() ?? [],
        };
    }

    private static Dictionary<string, Schema> Schemas { get; set; } = new();

    private static List<Models.DtoModels.Parameter> GetParameters(MethodInfo method)
    {
        var parameters = new List<Models.DtoModels.Parameter>();

        foreach (var p in method.GetParameters())
        {
            var fromBody = p.GetCustomAttribute<FromBodyAttribute>();
            var fromQuery = p.GetCustomAttribute<FromQueryAttribute>();
            var fromRoute = p.GetCustomAttribute<FromRouteAttribute>();

            var defaultIn = GetDefaultInFromMethod(method);

            var schemaName = p.ParameterType.Name!;
            if (!Schemas.ContainsKey(schemaName))
                Schemas[schemaName] = CreateSchema(p.ParameterType);

            var schema = Schemas[schemaName];

            var parameter = new Models.DtoModels.Parameter()
            {
                Name = p.Name!,
                In = fromBody != null ? "body" : fromQuery != null ? "query" : fromRoute != null ? "path" : defaultIn,
                Required = fromBody != null || fromRoute != null || p.Name == "id",
                Schema = schema,
            };

            parameters.Add(parameter);
        }

        return parameters;
    }

    private static Schema CreateSchema(Type type)
    {
        // if it was already created, return reference to it
        if (!IsComplex(type))
        {
            return new Schema()
            {
                Type = type.Name,
            };
        }

        if (Schemas.ContainsKey(type.Name))
        {
            return new Schema()
            {
                Reference = $"#/schemas/{type.GetType().Name}"
            };
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var props = properties.ToDictionary(
            prop => prop.Name,
            prop =>
            {
                var propType = UnwrapType(prop.PropertyType);

                if (Schemas.ContainsKey(propType.Name))
                    return Schemas[propType.Name];

                if (!IsComplex(propType))
                {
                    var req = propType.GetCustomAttribute<RequiredAttribute>() != null;
                    return new Schema()
                    {
                        Type = propType.Name,
                    };
                }
                
                if (propType.IsInterface)
                {
                    return new Schema()
                    {
                        Type = "INTERFACE",
                    };
                }

                if (IsList(propType))
                {
                    var type = GetListType(propType);
                    return new Schema()
                    {
                        IsList = true,
                        Type = type.Name,
                    };
                }

                if (IsDictionary(propType))
                {
                    var (key, value) = GetDictionaryTypes(propType);
                    return new Schema()
                    {
                        IsDictionary = true,
                        KeyType = key.Name,
                        ValueType = value.Name,
                    };
                }

                if (IsRangeFilter(propType, out bool nullable))
                {
                    var rangeType = GetRangeFilterType(propType);
                    return new Schema()
                    {
                        IsRange = true,
                        Type = rangeType.Name,
                        Required = [!nullable, !nullable],
                    };
                }

                // if it doesnt exist in the schemas, create it
                if (!Schemas.ContainsKey(propType.Name))
                    Schemas[propType.Name] = CreateSchema(propType);

                // always create a reference to the schema
                return new Schema()
                {
                    Type = propType.Name,
                    Reference = propType.IsClass ? $"#/schemas/{propType.Name}" : null
                };
            });

        var schema = new Schema()
        {
            Type = type.Name,
            Properties = props
        };

        Schemas[type.Name] = schema;
        return schema;
    }

    public static Type GetListType(Type type)
    {
        return type.GetGenericArguments().FirstOrDefault() ?? type;
    }

    public static bool IsList(Type type)
    {
        return type.IsGenericType && (
            type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
            type.GetGenericTypeDefinition() == typeof(List<>) ||
            type.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    public static bool IsDictionary(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }

    public static (Type, Type) GetDictionaryTypes(Type type)
    {
        var genericArgs = type.GetGenericArguments();
        if (genericArgs.Length != 2)
            throw new InvalidOperationException("Type is not a dictionary.");
        return (genericArgs[0], genericArgs[1]);
    }

    private static Response GetResponse(MethodInfo method)
    {
        var underlying = UnwrapType(method.ReturnType);

        if (!Schemas.ContainsKey(underlying.Name))
            Schemas[underlying.Name] = CreateSchema(underlying);

        return new Models.DtoModels.Response()
        {
            Content = new Dictionary<string, Schema>()
            {
                ["application/json"] = Schemas[underlying.Name]
            }
        };
    }

    private static string GetDefaultInFromMethod(MethodInfo method)
    {
        var http = method.GetCustomAttribute<HttpMethodAttribute>()!.HttpMethods.First();
        return http switch
        {
            "GET" => "query",
            "POST" => "body",
            "PUT" => "body",
            "DELETE" => "body",
            _ => throw new NotSupportedException($"HTTP method {http} is not supported.")
        };
    }

    private static bool IsRangeFilter(Type type, out bool nullable)
    {
        nullable = false;
        if (!type.IsGenericType) 
            return false;
        
        if (type.GetGenericTypeDefinition() == typeof(NullableRangeFilter<>))
        {
            nullable = true;
            return true;
        }

        if (type.GetGenericTypeDefinition() == typeof(RangeFilter<>))
        {
            return true;
        }

        return false;
    }

    private static Type GetRangeFilterType(Type type)
    {
        return type.GetGenericArguments().FirstOrDefault() ?? type;
    }

    private static Type UnwrapType(Type input)
    {
        if (!IsComplex(input)) return input;

        input = UnwrapTaskType(input);
        input = UnwrapActionResultType(input);
        input = UnwrapNullableType(input);

        return input;
    }

    private static Type UnwrapActionResultType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
        {
            return type.GetGenericArguments().First();
        }

        return type;
    }

    private static Type UnwrapTaskType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return type.GetGenericArguments().First();
        }

        return type;
    }

    private static Type UnwrapNullableType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return Nullable.GetUnderlyingType(type)!;
        }

        return type;
    }

    private static bool IsComplex(Type type)
    {
        return !type.IsPrimitive && type != typeof(string) && type != typeof(decimal) && type != typeof(DateTime);
    }
}


public static class FunctionExtensions
{
    public static Func<T, TResult> Compose<T, TIntermediate, TResult>(
        this Func<TIntermediate, TResult> f,
        Func<T, TIntermediate> g)
    {
        return x => f(g(x));
    }
}
