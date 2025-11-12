using KoliMate.Models;
using KoliMate.Services;
using KoliMate.ViewModels;
using KoliMate.Views;

namespace KoliMate
{
    public partial class App : Application
    {
        private readonly bool isLoggedIn;

        public App(IDatabaseService db, ICurrentUserService currentUserService)
        {
            InitializeComponent();
            Task.Run(async () => await db.InitAsync());

            isLoggedIn = Preferences.Get("IsLoggedIn", false);
            if (isLoggedIn)
            {
                Task.Run(async () => await currentUserService.InitializeAsync());
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            if (isLoggedIn)
            {
                
                return new Window(new AppShell());
            }
            else
            {
                var loginPage = MauiProgram.Services.GetService<LoginPage>();
                return new Window(new NavigationPage(loginPage));
            }
        }


    }
}