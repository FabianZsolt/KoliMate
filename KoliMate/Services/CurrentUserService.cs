using CommunityToolkit.Mvvm.ComponentModel;
using KoliMate.Models;

namespace KoliMate.Services
{
    public interface ICurrentUserService
    {
        User CurrentUser { get; set; }
    }

    public class CurrentUserService : ObservableObject, ICurrentUserService
    {
        private User currentUser;
        public User CurrentUser
        {
            get => currentUser;
            set => SetProperty(ref currentUser, value);
        }
    }
}
