using Microsoft.Extensions.Logging;
using KoliMate.Views;
using KoliMate.Models;
using KoliMate.ViewModels;
using CommunityToolkit.Maui;
using KoliMate.Services;


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

            // register current user service as singleton so every screen can access and modify
            builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginPageViewModel>();

            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfilePageViewModel>();

            builder.Services.AddTransient<MatchesPageViewModel>();

            builder.Services.AddTransient<SwipePageViewModel>();
            builder.Services.AddTransient<SwipePage>();

            // Matches page and viewmodel as singletons for tab reuse
            builder.Services.AddSingleton<MatchesPageViewModel>();
            builder.Services.AddSingleton<MatchesPage>();

            // Register MatchDetailPage and its ViewModel for DI
            builder.Services.AddTransient<MatchDetailPage>();
            builder.Services.AddTransient<MatchDetailViewModel>();

            // Register navigation service
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }

}

