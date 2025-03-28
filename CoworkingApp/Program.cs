using CoworkingApp.Data;
using CoworkingApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace CoworkingApp;

public static class Program
{
    private static Lock LogLock { get; } = new Lock();

    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // HERE IS THE CONFIGURATION OF THE APPLICATION (order doesnt matter)

        // Add services to the container.
        builder.Services.AddDbContext<CoworkingDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<WorkspacesService>();
        builder.Services.AddScoped<WorkspaceStatusesService>();
        builder.Services.AddScoped<CoworkingCentersService>();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoworkingApp", Version = "v1" }));

        // builder.Services.AddScoped<>

        var app = builder.Build();

        // HERE IS THE REQUEST HANDLING PIPELINE AKA MIDDLEWARES
        // Manipulates the request coming from the user or returning to the user.
        // Request go through each middleware from top to bottom.

        // Configure the HTTP request pipeline.

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthCore API V1");
            });
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.Use(async (HttpContext context, RequestDelegate next) =>
        {
            var start = DateTime.UtcNow;
            await next.Invoke(context);

            var duration = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            var requestLogEntry = $"RequestPath: {context.Request.Path}, Duration: {duration} ms";
                
            lock (LogLock)
            {
                File.AppendAllText("Logs/Requests.txt", requestLogEntry);
            }
        });

        app.UseHttpsRedirection();  
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
