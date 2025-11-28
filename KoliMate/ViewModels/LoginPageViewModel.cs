using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoliMate.Models;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using KoliMate.Services;

namespace KoliMate.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly IDatabaseService db;
        private readonly ICurrentUserService currentUserService;

        public LoginPageViewModel(IDatabaseService db, ICurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;

        }

        [ObservableProperty]
        string messageText;

        [ObservableProperty]
        Color messageTextColor;

        [ObservableProperty]
        string neptunEntryText;

        [ObservableProperty]
        User currentUser;

        [ObservableProperty]
        bool passwordEntryVisible;

        [ObservableProperty]
        string passwordEntryText;

        [ObservableProperty]
        string passwordLabel;

        [ObservableProperty]
        bool confirmEntryVisible;

        [ObservableProperty]
        string confirmEntryText;

        [ObservableProperty]
        bool nextButtonVisible = true;

        [RelayCommand]
        public async Task NextButton()
        {
            MessageText = "";
            string neptun = NeptunEntryText?.Trim().ToUpper();

            if (string.IsNullOrEmpty(neptun))
            {
                MessageText = "Add meg a Neptun-kódot.";
                return;
            }

            CurrentUser = await db.GetUserAsync(neptun);

            if (CurrentUser == null)
            {
                MessageText = "Nincs ilyen felhasználó az adatbázisban.";
                return;
            }

            // Ha megtaláltuk, mutassuk a jelszó mezőt
            PasswordEntryVisible = true;
            NextButtonVisible = false;

            if (CurrentUser.IsActive)
            {
                PasswordLabel = "Add meg a jelszavad:";
                ConfirmEntryVisible = false;
            }
            else
            {
                PasswordLabel = "Állíts be új jelszót:";
                ConfirmEntryVisible = true;
            }
        }

        [RelayCommand]
        public async Task PasswordSubmit()
        {
            string pwd = PasswordEntryText;
            string confirm = ConfirmEntryText;

            if (CurrentUser == null)
            {
                MessageText = "Előbb add meg a Neptun-kódot.";
                return;
            }

            if (CurrentUser.IsActive)
            {
                if (pwd == CurrentUser.Password)
                {
                    Preferences.Set("IsLoggedIn", true);
                    Preferences.Set("ShowProfilePrompt", true);

                    await currentUserService.ChangeCurrentUser(CurrentUser.NeptunCode ?? string.Empty);

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    MessageText = "Hibás jelszó.";
                }
            }
            else
            {
                // Új jelszó beállítása
                if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(confirm))
                {
                    MessageText = "Mindkét mezőt töltsd ki.";
                    return;
                }
                if (pwd != confirm)
                {
                    MessageText = "A jelszavak nem egyeznek.";
                    return;
                }

                CurrentUser.Password = pwd;
                CurrentUser.IsActive = true;
                await db.SaveUserAsync(CurrentUser);

                MessageTextColor = Colors.Green;
                MessageText = "Jelszó beállítva, most már beléphetsz.";
                PasswordLabel = "Add meg a jelszavad:";
                ConfirmEntryVisible = false;
            }
        }
    }
}
