using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KoliMate.Models
{
    public class SqliteDatabaseService : IDatabaseService
    {
        SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create;

        private readonly SQLiteAsyncConnection database;
        string dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "kolimate.db3");


        public SqliteDatabaseService()
        {
            database = new SQLiteAsyncConnection(dbPath, Flags);
        }

        public async Task InitAsync()
        {
            await database.CreateTableAsync<User>();
            await database.CreateTableAsync<RightSwipe>();
            // seed dummy data ha ures a tabla
            var users = await database.Table<User>().ToListAsync();
            if (users.Count == 0)
            {
                await database.InsertAsync(new User { Name = "Anna", BirthDate = new DateTime(2004, 5, 6), Description = "Student", PhotoPath = "", NeptunCode = "ABC123"});
                await database.InsertAsync(new User { Name = "Fábián Zsolt", BirthDate = new DateTime(2003, 07, 14), Description = "Mérnökinformatikus", PhotoPath = "", NeptunCode = "XMHZDW" });
            }
        }

        public async Task<List<User>> GetUsersAsync() => await database.Table<User>().ToListAsync();

        public async Task<User> GetUserAsync(string neptun) => await database.Table<User>().Where(u => u.NeptunCode == neptun).FirstOrDefaultAsync();

        public async Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
                return await database.UpdateAsync(user);
            else
                return await database.InsertAsync(user);
        }

        public async Task<int> DeleteUserAsync(User user) => await database.DeleteAsync(user);

        public async Task<List<RightSwipe>> GetRightSwipesAsync() => await database.Table<RightSwipe>().ToListAsync();

        public async Task<int> SaveRightSwipeAsync(RightSwipe swipe) => await database.InsertAsync(swipe);

        public async Task<int> UpdateRightSwipeAsync(RightSwipe swipe) => await database.UpdateAsync(swipe);


    }
}
