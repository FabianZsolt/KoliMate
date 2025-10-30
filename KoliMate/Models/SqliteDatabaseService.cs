using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KoliMate.App.Models
{
    public class SqliteDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public SqliteDatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "kolimate.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitAsync()
        {
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<RightSwipe>();
            // seed dummy data ha ures a tabla
            var users = await _database.Table<User>().ToListAsync();
            if (users.Count == 0)
            {
                await _database.InsertAsync(new User { Name = "Anna", Age = 22, Description = "Student", PhotoPath = "" });
                await _database.InsertAsync(new User { Name = "Peter", Age = 25, Description = "Engineer", PhotoPath = "" });
            }
        }

        public Task<List<User>> GetUsersAsync() => _database.Table<User>().ToListAsync();

        public Task<User> GetUserAsync(int id) => _database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();

        public Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
                return _database.UpdateAsync(user);
            else
                return _database.InsertAsync(user);
        }

        public Task<int> DeleteUserAsync(User user) => _database.DeleteAsync(user);

        public Task<List<RightSwipe>> GetRightSwipesAsync() => _database.Table<RightSwipe>().ToListAsync();

        public Task<int> SaveRightSwipeAsync(RightSwipe swipe) => _database.InsertAsync(swipe);

        public Task<int> UpdateRightSwipeAsync(RightSwipe swipe) => _database.UpdateAsync(swipe);
    }
}
