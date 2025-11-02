using Microsoft.Extensions.Logging;
using KoliMate.Views;
using KoliMate.Models;
using KoliMate.ViewModels;

namespace KoliMate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<SwipePage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddSingleton<IDatabaseService, SqliteDatabaseService>();
            builder.Services.AddTransient<MatchesPageViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();


        }
    }
}
