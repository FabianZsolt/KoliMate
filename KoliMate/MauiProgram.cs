using Microsoft.Extensions.Logging;
using KoliMate.Views;
using KoliMate.Models;
using KoliMate.ViewModels;
using CommunityToolkit.Maui;


namespace KoliMate
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Dependency injection
            builder.Services.AddSingleton<IDatabaseService, SqliteDatabaseService>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginPageViewModel>();

            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfilePageViewModel>();

            builder.Services.AddTransient<MatchesPageViewModel>();


            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }

}

