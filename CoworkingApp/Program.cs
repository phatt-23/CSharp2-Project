using System.Text;
using AutoMapper;
using CoworkingApp;
using CoworkingApp.Data;
using CoworkingApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
    


/////////////////////////////////////////////////////////////////////////////
// HERE IS THE CONFIGURATION OF THE APPLICATION (order doesnt matter)
// Add services to the container.
/////////////////////////////////////////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CoworkingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IMapper>(
    new MapperConfiguration(config => config.AddProfile<MapperProfile>())
        .CreateMapper()
    );

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoworkingApp", Version = "v1" }));
builder.Services.AddOpenApi("v1");

builder.Services.AddScoped<WorkspacesService>();
builder.Services.AddScoped<WorkspaceStatusesService>();
builder.Services.AddScoped<CoworkingCentersService>();
builder.Services.AddScoped<ReservationsService>();
builder.Services.AddScoped<IAuthService, AuthService>();


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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthCore API V1");
    });
    
    app.MapOpenApi();
    app.MapScalarApiReference();
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
