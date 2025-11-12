using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoliMate.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KoliMate.Services;
using System.Linq;

namespace KoliMate.ViewModels
{
    public partial class MatchesPageViewModel : ObservableObject
    {
        private readonly IDatabaseService databaseService;
        private readonly ICurrentUserService currentUserService;
        private readonly INavigationService navigationService;

        [ObservableProperty]
        private ObservableCollection<User> matches;

        public MatchesPageViewModel(IDatabaseService databaseService, ICurrentUserService currentUserService, INavigationService navigationService)
        {
            this.databaseService = databaseService;
            this.currentUserService = currentUserService;
            this.navigationService = navigationService;
            Matches = new ObservableCollection<User>();
            LoadMatchesCommand = new AsyncRelayCommand(LoadMatchesAsync);
            OpenMatchCommand = new AsyncRelayCommand<User>(OpenMatchAsync);
        }

        public IAsyncRelayCommand LoadMatchesCommand { get; }
        public IAsyncRelayCommand<User> OpenMatchCommand { get; }

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

        private async Task OpenMatchAsync(User user)
        {
            if (user == null)
                return;

            await navigationService.NavigateToMatchDetailAsync(user.Id);
        }
    }
}
