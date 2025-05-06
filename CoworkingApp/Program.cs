using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using AutoFilterer.Swagger;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Middlewares;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using CoworkingApp.Types;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;


namespace CoworkingApp;

public record ApiMethodTag(string Tag);

public enum ApiParameterBoundLocation
{ 
    Query,
    Body,
}

public class ApiParameterSchema
{

}

public class ApiComponentReference
{
    public required ApiObjectComponent Object { get; set; }
}

public class ApiObjectComponentProperty
{
    public required string Name { get; set; }
    public required ApiComponentReference Reference { get; set; }
    public Type? Type { get; set; }
    public string? Format { get; set; }
    public bool Nullable { get; set; } = false;
}

public class ApiObjectComponent 
{
    public required List<string> RequiredParameters { get; set; }
    public required List<ApiObjectComponentProperty> Properties { get; set; }
}

public class ApiMethodParameter 
{ 
    public required string Name { get; set; }
    public required ApiParameterBoundLocation Location { get; set; }
    public required ApiParameterSchema Schema { get; set; }
}

public class ApiResponse
{

}

public class ApiPathMethodDescription
{
    public required HttpMethod HttpMethod { get; set; }
    public required List<ApiMethodTag> Tags { get; set; }
    public required List<ApiMethodParameter> Parameters { get; set; }
    public required ApiResponse Response { get; set; }
}

public class ApiPathDescription 
{
    public required List<ApiPathMethodDescription> Methods { get; set; }
}
internal static class Program
{
    /// Extract the return type of the action method
    /// Takes of the Task<> and ActionResult<>
    /// Task<ActionResult<T>> => T
    private static Type UnwrapReturnType(Type type)
    {

        Type returnType = type;

        if (returnType.IsGenericType && typeof(Task).IsAssignableFrom(returnType))
        {
            returnType = returnType.GetGenericArguments().FirstOrDefault() ?? returnType;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
        {
            returnType = returnType.GetGenericArguments().FirstOrDefault() ?? returnType;
        }

        return returnType;
    }

    private static bool IsComplexType(Type type)
    {
        if (type.IsPrimitive
            || type == typeof(string)
            || type == typeof(DateTime)
            || type == typeof(decimal))
        { 
            return false;
        }

        if (type.IsGenericType && 
            type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return IsComplexType(Nullable.GetUnderlyingType(type)!);
        }

        return true;
    }

    private static void GetJsonSpecification()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var apiControllers = assembly
            .GetTypes()
            .Where(t => t.IsClass && t.GetCustomAttribute<ApiControllerAttribute>() != null);

        foreach (var controller in apiControllers)
        {
            Console.WriteLine(controller.Name);

            var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var action in actions)
            {
                Console.WriteLine("\t" + "Action: " + action.Name);
                var parameters = action.GetParameters();
                foreach(var parameter in parameters) 
                { 
                    Console.WriteLine(
                        "\t\t" + 
                        parameter.Name + ": " + 
                        parameter.ParameterType.Name);

                    if (IsComplexType(parameter.ParameterType))
                    {

                    }
                }

                Console.WriteLine("\t\t=>" + action.ReturnType.Name);
                Console.WriteLine("\t\t=>" + UnwrapReturnType(action.ReturnType).Name);
            }
        }

    }

    private static void Main(string[] args)
    {
        //GetJsonSpecification();
        //return;

        /////////////////////////////////////////////////////////////////////////////
        // Services /////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi("v1");  // [server-website]/openapi/v1.json
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoworkingApp", Version = "v1" });
            c.UseAutoFiltererParameters();
        });

        builder.Services
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(typeof(Program).Assembly)
            .AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });

        builder.Services.AddHttpClient();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.AccessDeniedPath = "account/login";  
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"]!,
                    
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"]!,
                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
                        
                    ValidateLifetime = true,
                };
                
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = (context) =>
                    {
                        // Extract JWT token from cookie if Authorization header is not set
                        var accessTokenFromCookie = context.Request.Cookies["jwt"];
                        
                        // If the cookie was found, then use this cookie
                        // if not then use Authorization header with the bearer token
                        // (this is done automatically btw, it's the standard of JWT)
                        if (!string.IsNullOrEmpty(accessTokenFromCookie))
                        {
                            context.Token = accessTokenFromCookie;
                        }
                        
                        return Task.CompletedTask;
                    }
                };    
            });

        builder.Services.AddDbContext<CoworkingDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSingleton(new MapperConfiguration(config => config.AddProfile<MapperProfile>()).CreateMapper());

        builder.Services
            .AddHttpClient<IGeocodingService, NominatimGeocodingService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        builder.Services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        builder.Services.AddScoped<IWorkspaceHistoryRepository, WorkspaceWorkspaceHistoryRepository>();
        builder.Services.AddScoped<IWorkspaceStatusRepository, WorkspaceStatusRepository>();
        builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        builder.Services.AddScoped<IWorkspaceStatusRepository, WorkspaceStatusRepository>();
        builder.Services.AddScoped<ICoworkingCenterRepository, CoworkingCenterRepository>();
        builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
        builder.Services.AddScoped<IWorkspacePricingRepository, WorkspacePricingRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        builder.Services.AddScoped<IAddressRepository, AddressRepository>();

        builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
        builder.Services.AddScoped<IWorkspaceStatusService, WorkspaceStatusService>();
        builder.Services.AddScoped<IReservationService, ReservationService>();
        builder.Services.AddScoped<ICoworkingCenterService, CoworkingCenterService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IWorkspacePricingService, WorkspacePricingService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAddressService, AddressService>();

        var app = builder.Build();

        // For PostgresDB timestamp to compatible with C# datetime
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


        //////////////////////////////////////////////////////////////////////////
        // Middlewares ///////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////

        // In the development profile, use the Swagger and Scalar for API testing.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthCore API V1");
            });
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseMiddleware<TokenRefreshMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }


}