using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KoliMate.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using KoliMate.Services;

namespace KoliMate.ViewModels
{
    public partial class SwipePageViewModel : ObservableObject
    {
        private readonly IDatabaseService databaseService;
        private readonly ICurrentUserService currentUserService;

        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private string noUsersMessage = string.Empty;

        private List<User> allUsers;
        private int currentIndex = 0;

        // ez a változó nem engedi meg, hogy a Like vagy Dislike művelet egyszerre többször fusson
        private bool isProcessing;

        public SwipePageViewModel(IDatabaseService databaseService, ICurrentUserService currentUserService)
        {
            this.databaseService = databaseService;
            this.currentUserService = currentUserService;

            LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
            LikeCommand = new AsyncRelayCommand(OnLikeAsync, () => CurrentUser != null && !isProcessing);
            DislikeCommand = new RelayCommand(OnDislike, () => CurrentUser != null && !isProcessing);
        }

        public AsyncRelayCommand LoadUsersCommand { get; }
        public AsyncRelayCommand LikeCommand { get; }
        public RelayCommand DislikeCommand { get; }

        private async Task LoadUsersAsync()
        {
            currentIndex = 0;
            allUsers = await databaseService.GetUsersAsync();
            // kizárjuk az aktuális felhasználót
            var signedInId = currentUserService.CurrentUser?.Id ?? -1;
            var likedSwipes = await databaseService.GetRightSwipesAsync();
            var likedIds = likedSwipes.Where(s => s.LikerId == signedInId).Select(s => s.LikedId).ToHashSet();

            allUsers = allUsers.Where(u => u.Id != signedInId && u.IsActive && !likedIds.Contains(u.Id)).ToList();

            if (allUsers == null || allUsers.Count == 0)
            {
                CurrentUser = null; 
                return;
            }

            LoadNextUser();
        }

        private void LoadNextUser()
        {
            if (allUsers != null && currentIndex < allUsers.Count)
                CurrentUser = allUsers[currentIndex++];
            else
                CurrentUser = null;
        }

        private async Task OnLikeAsync()
        {
            if (CurrentUser == null) return;

            if (isProcessing) return;
            isProcessing = true;
            LikeCommand.NotifyCanExecuteChanged();
            DislikeCommand.NotifyCanExecuteChanged();

            try
            {
                var swipes = await databaseService.GetRightSwipesAsync() ?? new List<RightSwipe>();

                var signedInId = currentUserService.CurrentUser?.Id ?? -1;

                // megnézzük, hogy a másik fél már like-olt-e minket
                var existing = swipes.FirstOrDefault(s => s.LikerId == CurrentUser.Id && s.LikedId == signedInId);

                if (existing != null)
                {
                    existing.IsMatch = true;
                    await databaseService.UpdateRightSwipeAsync(existing);

                    await databaseService.SaveRightSwipeAsync(new RightSwipe
                    {
                        LikerId = signedInId,
                        LikedId = CurrentUser.Id,
                        IsMatch = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    await databaseService.SaveRightSwipeAsync(new RightSwipe
                    {
                        LikerId = signedInId,
                        LikedId = CurrentUser.Id,
                        IsMatch = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // tovább lépünk a következő felhasználóra
                LoadNextUser();
            }
            finally
            {
                isProcessing = false;
                LikeCommand.NotifyCanExecuteChanged();
                DislikeCommand.NotifyCanExecuteChanged();
            }
        }

        private void OnDislike()
        {
            if (isProcessing) return;

            isProcessing = true;
            LikeCommand.NotifyCanExecuteChanged();
            DislikeCommand.NotifyCanExecuteChanged();

            try
            {
                LoadNextUser();
            }
            finally
            {
                isProcessing = false;
                LikeCommand.NotifyCanExecuteChanged();
                DislikeCommand.NotifyCanExecuteChanged();
            }
        }

        partial void OnCurrentUserChanged(User value)
        {
            // jelezzük, ha elfogytak a felhasználók
            NoUsersMessage = value == null ? "No more users available." : string.Empty;

            LikeCommand.NotifyCanExecuteChanged();
            DislikeCommand.NotifyCanExecuteChanged();
        }
    }
}
