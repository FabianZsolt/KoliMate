using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace KoliMate.Services
{
    public interface INavigationService
    {
        Task NavigateToMatchDetailAsync(int userId);
    }

    public class NavigationService : INavigationService
    {
        public async Task NavigateToMatchDetailAsync(int userId)
        {
            // using Shell route with query
            var route = $"{nameof(KoliMate.Views.MatchDetailPage)}?userId={userId}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
