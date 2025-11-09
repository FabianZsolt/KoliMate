using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoliMate.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KoliMate.Services;

namespace KoliMate.ViewModels
{
    public partial class MatchesPageViewModel : ObservableObject
    {
        private readonly IDatabaseService databaseService;
        private readonly ICurrentUserService currentUserService;

        [ObservableProperty]
        private ObservableCollection<User> matches;

        public MatchesPageViewModel(IDatabaseService databaseService, ICurrentUserService currentUserService)
        {
            this.databaseService = databaseService;
            this.currentUserService = currentUserService;
            Matches = new ObservableCollection<User>();
            LoadMatchesCommand = new AsyncRelayCommand(LoadMatchesAsync);
        }

        public IAsyncRelayCommand LoadMatchesCommand { get; }

        private async Task LoadMatchesAsync()
        {
            Matches.Clear();

            var allUsers = await databaseService.GetUsersAsync();
            var rightSwipes = await databaseService.GetRightSwipesAsync();

            var signedInId = currentUserService.CurrentUser?.Id ?? -1;

            foreach (var user in allUsers)
            {
                if (user.Id == signedInId)
                    continue;

                bool userLikedMe = rightSwipes.Any(s => s.LikedId == signedInId && s.LikerId == user.Id);
                bool iLikedUser = rightSwipes.Any(s => s.LikerId == signedInId && s.LikedId == user.Id);

                if (userLikedMe && iLikedUser)
                    Matches.Add(user);
            }
        }
    }
}
