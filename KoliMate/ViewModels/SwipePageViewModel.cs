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
        private readonly IDatabaseService _databaseService;
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private string noUsersMessage = string.Empty;

        private List<User> allUsers;
        private int currentIndex = 0;

        // Prevent concurrent swipe handling which was causing state races and crashes
        private bool isProcessing;

        public SwipePageViewModel(IDatabaseService databaseService, ICurrentUserService currentUserService)
        {
            _databaseService = databaseService;
            _currentUserService = currentUserService;

            // commands' CanExecute now also checks `isProcessing` to prevent concurrent execution/race conditions
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
            allUsers = await _databaseService.GetUsersAsync();
            // kizárjuk az aktuális felhasználót
            var signedInId = _currentUserService.CurrentUser?.Id ?? -1;
            allUsers = allUsers.Where(u => u.Id != signedInId).ToList();

            if (allUsers == null || allUsers.Count == 0)
            {
                CurrentUser = null; // triggers message via OnCurrentUserChanged
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

            // guard to avoid concurrent runs (double-tap or rapid swipes)
            if (isProcessing) return;
            isProcessing = true;
            LikeCommand.NotifyCanExecuteChanged();
            DislikeCommand.NotifyCanExecuteChanged();

            try
            {
                var swipes = await _databaseService.GetRightSwipesAsync() ?? new List<RightSwipe>();

                var signedInId = _currentUserService.CurrentUser?.Id ?? -1;

                // check whether the other user (CurrentUser) already liked the signed-in user
                var existing = swipes.FirstOrDefault(s => s.LikerId == CurrentUser.Id && s.LikedId == signedInId);

                if (existing != null)
                {
                    existing.IsMatch = true;
                    await _databaseService.UpdateRightSwipeAsync(existing);

                    await _databaseService.SaveRightSwipeAsync(new RightSwipe
                    {
                        LikerId = signedInId,
                        LikedId = CurrentUser.Id,
                        IsMatch = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    await _databaseService.SaveRightSwipeAsync(new RightSwipe
                    {
                        LikerId = signedInId,
                        LikedId = CurrentUser.Id,
                        IsMatch = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // advance to next user only after DB work is complete
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

            // mark processing briefly to prevent very fast repeated dislikes
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

        // Called by the source generator when CurrentUser changes.
        partial void OnCurrentUserChanged(User value)
        {
            // update UI message when no users remain
            NoUsersMessage = value == null ? "No more users available." : string.Empty;

            // update command availability
            LikeCommand.NotifyCanExecuteChanged();
            DislikeCommand.NotifyCanExecuteChanged();
        }
    }
}
