using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace KoliMate
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Preferences.Get("ShowProfilePrompt", false))
            {
                // clear flag so it only shows once
                Preferences.Set("ShowProfilePrompt", false);

                // small delay to ensure Shell is ready
                await Task.Delay(200);

                // navigate to the Profile tab (shell route defined as "profile")
                await GoToAsync("//profile");

                // show a message asking the user to fill in their data
                await DisplayAlert("Profil", "Kérlek töltsd ki a profilodat.", "Rendben");
            }
        }
    }
}
