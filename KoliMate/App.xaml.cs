using KoliMate.Models;
using KoliMate.ViewModels;
using KoliMate.Views;

namespace KoliMate
{
    public partial class App : Application
    {
        private readonly bool isLoggedIn;

        public App(IDatabaseService db)
        {
            InitializeComponent();
            Task.Run(async () => await db.InitAsync());

            isLoggedIn = Preferences.Get("IsLoggedIn", false);
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