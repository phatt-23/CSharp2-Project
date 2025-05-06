using AutoMapper;
using CoworkingDesktop.Helpers;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels;
using CoworkingDesktop.Views;
using CoworkingDesktop.Views.Dialogs;
using CoworkingDesktop.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace CoworkingDesktop
{
    public partial class App : Application
    {
        public static readonly string RESOURCE_URL = "http://localhost:5176";
        public static readonly string CREDS_TOKENS_PATH = "secret_creds.txt";
        public static readonly string API_HTTP_CLIENT = "ApiClient";

        public CookieContainer AppCookies = new();
        public IServiceProvider Services { get; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Http API client setup

            // This auth service has its own HttpClient,
            // because the ApiClient's AuthHeadersHandler
            // need this AuthService to get the token
            services.AddSingleton<IAuthService, AuthService>();

            services.AddTransient<AuthHeadersHandler>();

            services.AddHttpClient(API_HTTP_CLIENT, client =>
            {
                client.BaseAddress = new Uri(RESOURCE_URL);
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                CookieContainer = AppCookies,
                UseCookies = true,
            })
            .AddHttpMessageHandler<AuthHeadersHandler>();

            // API Services
            services.AddScoped<INavigationService, NavigationService>();
            services.AddScoped<IReservationsService, ReservationsService>();
            services.AddScoped<IWorkspacesService, WorkspacesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<ICoworkingCenterService, CoworkingCenterService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IStatsService, StatsService>();

            // General Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddScoped<IDialogService, DialogService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddSingleton<ReservationsViewModel>();
            services.AddSingleton<WorkspacesViewModel>();
            services.AddSingleton<CoworkingCentersViewModel>();
            services.AddSingleton<UsersViewModel>();
            services.AddSingleton<ReservationFormView>();
            services.AddSingleton<StatsViewModel>();

            // Views and Windows
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginWindow>();

            services.AddSingleton<ReservationsView>();
            services.AddSingleton<WorkspacesView>();
            services.AddSingleton<CoworkingCentersView>();
            services.AddSingleton<UsersView>();

            services.AddTransient<ReservationFormViewModel>();

            // AutoMapper
            services.AddSingleton(new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var login = Services.GetRequiredService<LoginWindow>();
            login.Show();
        }
    }
}

