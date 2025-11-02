using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using KoliMate.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Alerts;

namespace KoliMate.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IDatabaseService db;

        public ProfilePageViewModel(IDatabaseService db)
        {
            this.db = db;
            currentUser = new User(); // ideiglenes üres user
            BirthDate = DateTime.Today;
            MaxDate = DateTime.Today;
        }

        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private DateTime birthDate;

        public DateTime MaxDate { get; private set; }

        public async Task InitAsync()
        {
            var user = await db.GetUserAsync(Preferences.Default.Get("currentUserNeptun", ""));
            if (user != null)
                CurrentUser = user;
        }

        partial void OnCurrentUserChanged(User value)
        {
            if (value != null)
            {
                BirthDate = value.BirthDate == default ? DateTime.Today : value.BirthDate;
            }
        }

        partial void OnBirthDateChanged(DateTime value)
        {
            if (CurrentUser != null)
            {
                CurrentUser.BirthDate = value;
            }
        }

        [RelayCommand]
        public async Task Save()
        {
            // Ensure CurrentUser.BirthDate is synced before saving
            if (CurrentUser != null)
                CurrentUser.BirthDate = BirthDate;

            var rows = await db.SaveUserAsync(CurrentUser);

            // if save succeeded, show a snackbar (gray bar at the bottom)
            if (rows > 0)
            {
                try
                {
                    await Snackbar.Make("Profil mentése sikeres.", duration: TimeSpan.FromSeconds(3)).Show();
                }
                catch
                {
                    // ignore if Snackbar isn't available at runtime
                }
            }
        }
    }
}
