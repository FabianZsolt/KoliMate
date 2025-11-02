using KoliMate.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoliMate.ViewModels
{
    public class MatchesPageViewModel
    {
        private readonly IDatabaseService db;

        public MatchesPageViewModel(IDatabaseService db)
        {
            this.db = db;
        }


        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public async Task InitAsync()
        {
            var users = await db.GetUsersAsync();
            Users.Clear();
            if (users != null)
            {
                foreach (var u in users)
                    Users.Add(u);
            }
        }
    }
}
