using KoliMate.Models;
using KoliMate.Views;

namespace KoliMate
{
    public partial class App : Application
    {
        private readonly bool _isLoggedIn;

        public App(IDatabaseService db)
        {
            InitializeComponent();
            Task.Run(async () => await db.InitAsync());

            _isLoggedIn = Preferences.Get("IsLoggedIn", false);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            if (_isLoggedIn)
            {
                // Normál app (Shell) betöltése
                return new Window(new AppShell());
            }
            else
            {
                // Bejelentkezési oldal megjelenítése
                return new Window(new NavigationPage(new LoginPage()));
            }
        }
    }
}