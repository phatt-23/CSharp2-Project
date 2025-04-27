using System.Text;
using AutoFilterer.Swagger;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Middlewares;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;


namespace CoworkingApp;

internal static class Program
{

    private static void Main(string[] args)
    {
        /////////////////////////////////////////////////////////////////////////////
        // HERE IS THE CONFIGURATION OF THE APPLICATION (order doesn't matter)
        // Add services to the container.
        /////////////////////////////////////////////////////////////////////////////

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<CoworkingDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSingleton(
            new MapperConfiguration(config => config.AddProfile<MapperProfile>())
                .CreateMapper()
        );

        builder.Services
            .AddFluentValidationClientsideAdapters()
            .AddFluentValidationAutoValidation(config => {
                config.DisableDataAnnotationsValidation = true;
            })
            .AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddHttpClient();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                            context.Token = accessTokenFromCookie;
                        
                        return Task.CompletedTask;
                    }
                };    
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoworkingApp", Version = "v1" });
            c.UseAutoFiltererParameters();
        });
        builder.Services.AddOpenApi("v1");  // [server-website]/openapi/v1.json




        builder.Services.AddScoped<IWorkspaceHistoryRepository, WorkspaceWorkspaceHistoryRepository>();
        builder.Services.AddScoped<IWorkspaceStatusRepository, WorkspaceStatusRepository>();

        builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
        builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();

        builder.Services.AddScoped<IWorkspaceStatusService, WorkspaceStatusService>();
        builder.Services.AddScoped<IWorkspaceStatusRepository, WorkspaceStatusRepository>();

        builder.Services.AddScoped<IReservationService, ReservationService>();
        builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

        builder.Services.AddScoped<ICoworkingCenterRepository, CoworkingCenterRepository>();
        builder.Services.AddScoped<ICoworkingCenterService, CoworkingCenterService>();

        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped<IWorkspacePricingService, WorkspacePricingService>();
        builder.Services.AddScoped<IWorkspacePricingRepository, WorkspacePricingRepository>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();




        var app = builder.Build();


        // For PostgresDB timestamp to compatible with C# datetime
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


        //////////////////////////////////////////////////////////////////////////
        // HERE IS THE REQUEST HANDLING PIPELINE (AKA MIDDLEWARES)
        // Manipulates the request coming from the user or returning to the user.
        // Request go through each middleware from top to bottom.
        // Configure the HTTP request pipeline.
        //////////////////////////////////////////////////////////////////////////

        // In the development profile, use the Swagger and Scalar for API testing.
        app.UseMiddleware<TokenRefreshMiddleware>();

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