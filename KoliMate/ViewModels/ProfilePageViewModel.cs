using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using KoliMate.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using System.IO;
using Microsoft.Maui.Controls;
using KoliMate.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel.DataTransfer;


namespace KoliMate.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IDatabaseService db;

        public ProfilePageViewModel(IDatabaseService db)
        {
            this.db = db;
            currentUser = new User();
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
            var neptun = Preferences.Default.Get("currentUserNeptun", "");
            if (!string.IsNullOrEmpty(neptun))
            {
                var user = await db.GetUserAsync(neptun);
                if (user != null)
                    CurrentUser = user;
            }
        }

        partial void OnCurrentUserChanged(User value)
        {
            if (value != null)
                BirthDate = value.BirthDate == default ? DateTime.Today : value.BirthDate;
        }

        partial void OnBirthDateChanged(DateTime value)
        {
            if (CurrentUser != null)
                CurrentUser.BirthDate = value;
        }

        [RelayCommand]
        public async Task PickPhotoAsync()
        {
            try
            {
                var image = await MediaPicker.Default.PickPhotoAsync();
                if (image == null) return;

                string localPath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{Guid.NewGuid()}_{image.FileName}");

                using Stream src = await image.OpenReadAsync();
                using FileStream dest = File.OpenWrite(localPath);
                await src.CopyToAsync(dest);

                CurrentUser.PhotoPath = localPath;
                await db.SaveUserAsync(CurrentUser);
            }
            catch (Exception ex)
            {
                await Snackbar.Make($"Nem sikerült a kép betöltése: {ex.Message}").Show();
            }
        }

        [RelayCommand]
        public async Task TakePhotoAsync()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Snackbar.Make("A kamerás fényképezés nem támogatott ezen az eszközön.").Show();
                    return;
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null) return;

                string localPath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{Guid.NewGuid()}_{photo.FileName}");

                using Stream src = await photo.OpenReadAsync();
                using FileStream dest = File.OpenWrite(localPath);
                await src.CopyToAsync(dest);

                CurrentUser.PhotoPath = localPath;
                await db.SaveUserAsync(CurrentUser);
            }
            catch (Exception ex)
            {
                await Snackbar.Make($"Nem sikerült a fényképezés: {ex.Message}").Show();
            }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (CurrentUser != null)
                CurrentUser.BirthDate = BirthDate;

            var rows = await db.SaveUserAsync(CurrentUser);

            if (rows > 0)
                await Snackbar.Make("Profil mentése sikeres.", duration: TimeSpan.FromSeconds(3)).Show();
        }

        [RelayCommand]
        public async Task SignOut()
        {
            var page = Application.Current?.MainPage;
            if (page == null)
                return;

            bool confirm = await page.DisplayAlert("Kijelentkezés", "Biztosan kijelentkezel?", "Igen", "Mégse");
            if (!confirm)
                return;

            Preferences.Set("IsLoggedIn", false);
            Preferences.Remove("currentUserNeptun");
            Preferences.Set("ShowProfilePrompt", false);

            // Navigate to LoginPage using DI to resolve the page/viewmodel
            var serviceProvider = MauiProgram.Services;
            var loginPage = serviceProvider.GetService<LoginPage>() ?? new LoginPage(serviceProvider.GetRequiredService<LoginPageViewModel>());

            Application.Current.MainPage = loginPage;
        }


        [RelayCommand]
        public async Task ShareProfileAsync()
        {
            if (CurrentUser == null)
            {
                await Snackbar.Make("Nincs betöltött profil.").Show();
                return;
            }

            // Összeállítjuk a megosztandó szöveget
            string message = "Én már regisztráltam a KoliMate-re!" +
                             $"👤 {CurrentUser.Name}\n" +
                             $"🎂 Születési dátum: {CurrentUser.BirthDate:d}\n" +
                             $"💬 Rólam: {CurrentUser.Description}";

            // Megosztás
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = message,
                Title = "KoliMate profil megosztása"
            });
        }

    }
}
