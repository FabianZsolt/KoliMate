using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoliMate.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoliMate.ViewModels
{
    public partial class SwipePageViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;

        [ObservableProperty]
        private User currentUser;

        private List<User> allUsers;
        private int currentIndex = 0;

        public SwipePageViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
            LikeCommand = new AsyncRelayCommand(OnLikeAsync);
            DislikeCommand = new RelayCommand(OnDislike);
        }

        public IAsyncRelayCommand LoadUsersCommand { get; }
        public IAsyncRelayCommand LikeCommand { get; }
        public IRelayCommand DislikeCommand { get; }

        private int currentUserId = 2; // pl. bejelentkezett felhasználó (később: App.CurrentUser.Id)

        private async Task LoadUsersAsync()
        {
            allUsers = await _databaseService.GetUsersAsync();
            // kizárjuk az aktuális felhasználót
            allUsers = allUsers.Where(u => u.Id != currentUserId).ToList();
            LoadNextUser();
        }

        private void LoadNextUser()
        {
            if (currentIndex < allUsers.Count)
                CurrentUser = allUsers[currentIndex++];
            else
                CurrentUser = null;
        }

        private async Task OnLikeAsync()
        {
            if (CurrentUser == null) return;

            var existing = (await _databaseService.GetRightSwipesAsync())
                .FirstOrDefault(s => s.LikerId == CurrentUser.Id && s.LikedId == currentUserId);

            if (existing != null)
            {
                existing.IsMatch = true;
                await _databaseService.UpdateRightSwipeAsync(existing);

                await _databaseService.SaveRightSwipeAsync(new RightSwipe
                {
                    LikerId = currentUserId,
                    LikedId = CurrentUser.Id,
                    IsMatch = true
                });
            }
            else
            {
                await _databaseService.SaveRightSwipeAsync(new RightSwipe
                {
                    LikerId = currentUserId,
                    LikedId = CurrentUser.Id,
                    IsMatch = false
                });
            }

            LoadNextUser();
        }

        private void OnDislike()
        {
            LoadNextUser();
        }
    }
}
