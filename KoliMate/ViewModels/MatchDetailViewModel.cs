using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoliMate.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;
using System.Linq;
using KoliMate.Services;

namespace KoliMate.ViewModels
{
    public partial class MatchDetailViewModel : ObservableObject
    {
        private readonly IDatabaseService databaseService;

        [ObservableProperty]
        private User user;

        public MatchDetailViewModel(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;

            SaveContactCommand = new AsyncRelayCommand(SaveContactAsync);
            CopyPhoneCommand = new AsyncRelayCommand(CopyPhoneAsync);
            CopyEmailCommand = new AsyncRelayCommand(CopyEmailAsync);
            CopyLinkCommand = new AsyncRelayCommand(CopyLinkAsync);

            OpenPhoneCommand = new AsyncRelayCommand(OpenPhoneAsync);
            OpenEmailCommand = new AsyncRelayCommand(OpenEmailAsync);
            OpenLinkCommand = new AsyncRelayCommand(OpenLinkAsync);
        }

        public IAsyncRelayCommand SaveContactCommand { get; }
        public IAsyncRelayCommand CopyPhoneCommand { get; }
        public IAsyncRelayCommand CopyEmailCommand { get; }
        public IAsyncRelayCommand CopyLinkCommand { get; }
        public IAsyncRelayCommand OpenPhoneCommand { get; }
        public IAsyncRelayCommand OpenEmailCommand { get; }
        public IAsyncRelayCommand OpenLinkCommand { get; }

        public async Task LoadUserAsync(int id)
        {
            var users = await databaseService.GetUsersAsync();
            User = users.FirstOrDefault(x => x.Id == id);
        }

        private async Task CopyPhoneAsync()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.PhoneNumber)) return;
            await Clipboard.Default.SetTextAsync(User.PhoneNumber);
        }

        private async Task CopyEmailAsync()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.Email)) return;
            await Clipboard.Default.SetTextAsync(User.Email);
        }

        private async Task CopyLinkAsync()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.FacebookProfile)) return;
            await Clipboard.Default.SetTextAsync(User.FacebookProfile);
        }

        private async Task SaveContactAsync()
        {
            if (User == null) return;
            try
            {
#if ANDROID
                var intent = new Android.Content.Intent(Android.Provider.ContactsContract.Intents.Insert.Action);
                intent.SetType(Android.Provider.ContactsContract.RawContacts.ContentType);
                if (!string.IsNullOrWhiteSpace(User.Name))
                    intent.PutExtra(Android.Provider.ContactsContract.Intents.Insert.Name, User.Name);

                if (!string.IsNullOrWhiteSpace(User.PhoneNumber))
                    intent.PutExtra(Android.Provider.ContactsContract.Intents.Insert.Phone, User.PhoneNumber);

                if (!string.IsNullOrWhiteSpace(User.Email))
                    intent.PutExtra(Android.Provider.ContactsContract.Intents.Insert.Email, User.Email);

                intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
#else
                var vcard = "BEGIN:VCARD\nVERSION:3.0\n";
                if (!string.IsNullOrWhiteSpace(User.Name)) vcard += $"FN:{User.Name}\n";
                if (!string.IsNullOrWhiteSpace(User.PhoneNumber)) vcard += $"TEL;TYPE=CELL:{User.PhoneNumber}\n";
                if (!string.IsNullOrWhiteSpace(User.Email)) vcard += $"EMAIL;TYPE=INTERNET:{User.Email}\n";
                if (!string.IsNullOrWhiteSpace(User.FacebookProfile)) vcard += $"URL:{User.FacebookProfile}\n";
                vcard += "END:VCARD\n";

                await Clipboard.Default.SetTextAsync(vcard);
                // Note: view is responsible for showing alerts; keeping ViewModel testable.
#endif
            }
            catch (Exception)
            {
                
            }
        }

        private async Task OpenPhoneAsync()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.PhoneNumber)) return;
            try
            {
                if (PhoneDialer.Default.IsSupported)
                    PhoneDialer.Default.Open(User.PhoneNumber);
            }
            catch { }
        }

        private async Task OpenEmailAsync()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.Email)) return;
            try
            {
                var message = new EmailMessage { To = new List<string> { User.Email } };
                await Email.Default.ComposeAsync(message);
            }
            catch { }
        }

        private async Task OpenLinkAsync()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.FacebookProfile)) return;
            try
            {
                var url = User.FacebookProfile;
                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    url = "https://" + url;
                await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(url);
            }
            catch { }
        }
    }
}
