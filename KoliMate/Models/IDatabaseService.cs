using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoliMate.Models
{
    public interface IDatabaseService
    {
        Task InitAsync();
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(string neptun);
        Task<int> SaveUserAsync(User user);
        Task<int> DeleteUserAsync(User user);

        Task<List<RightSwipe>> GetRightSwipesAsync();
        Task<int> SaveRightSwipeAsync(RightSwipe swipe);
        Task<int> UpdateRightSwipeAsync(RightSwipe swipe);
    }
}
