using Microsoft.Extensions.Logging;
using KoliMate.Views;
using KoliMate.Models;

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
            builder.Services.AddSingleton<IDatabaseService, SqliteDatabaseService>();
            return builder.Build();


        }
    }
}
