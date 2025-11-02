using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using KoliMate.Models;
using CommunityToolkit.Mvvm.Input;

namespace KoliMate.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IDatabaseService db;

        public ProfilePageViewModel(IDatabaseService db)
        {
            this.db = db;
            currentUser = new User(); // ideiglenes üres user
        }

        [ObservableProperty]
        private User currentUser;

        public async Task InitAsync()
        {
            var user = await db.GetUserAsync(Preferences.Default.Get("currentUserNeptun", "XMHZDW"));
            if (user != null)
                CurrentUser = user;
        }

        [RelayCommand]
        public async Task Save()
        {
            await db.SaveUserAsync(CurrentUser);
        }
    }
}
