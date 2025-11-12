using CommunityToolkit.Mvvm.ComponentModel;
using KoliMate.Models;

namespace KoliMate.Services
{
    public interface ICurrentUserService
    {
        User? CurrentUser { get; }
        Task ChangeCurrentUser(string neptun);
        Task InitializeAsync();
    }

    public class CurrentUserService : ObservableObject, ICurrentUserService
    {
        private const string PreferenceKey = "currentUserNeptun";

        IDatabaseService db;

        public CurrentUserService(IDatabaseService db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Read-only property that returns the cached current user (may be null until initialized).
        public User? CurrentUser
        {
            get;
            private set;
        }

        // Loads the current user from preferences and database. Call this at app startup.
        public async Task InitializeAsync()
        {
            var neptun = Preferences.Default.Get(PreferenceKey, "");
            if (string.IsNullOrEmpty(neptun))
            {
                CurrentUser = null;
                return;
            }

            CurrentUser = await db.GetUserAsync(neptun);
        }

        // Change the current user: update the stored preference and load the user from the database.
        public async Task ChangeCurrentUser(string neptun)
        {
            neptun = neptun ?? string.Empty;
            Preferences.Default.Set(PreferenceKey, neptun);

            if (string.IsNullOrEmpty(neptun))
            {
                CurrentUser = null;
                return;
            }

            CurrentUser = await db.GetUserAsync(neptun);
        }
    }
}
