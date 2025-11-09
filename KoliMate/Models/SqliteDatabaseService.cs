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
                var seedUsers = new List<User>
                {
                    new User { Name = "Anna", BirthDate = new DateTime(2004, 5, 6), Description = "Student", PhotoPath = "avatars/anna.png", NeptunCode = "ABC123", Password = "pass1", IsActive = true },
                    new User { Name = "Fábián Zsolt", BirthDate = new DateTime(2003, 7, 14), Description = "Mérnökinformatikus", PhotoPath = "avatars/fabian.png", NeptunCode = "XMHZDW", Password = "pass2", IsActive = true },
                    new User { Name = "Bence Kovács", BirthDate = new DateTime(1999, 11, 3), Description = "Designer", PhotoPath = "avatars/bence.png", NeptunCode = "BNC456", Password = "pass3", IsActive = true },
                    new User { Name = "Eszter Nagy", BirthDate = new DateTime(2001, 2, 18), Description = "Product Manager", PhotoPath = "avatars/eszter.png", NeptunCode = "ESZ789", Password = "pass4", IsActive = true },
                    new User { Name = "Dóra Szabó", BirthDate = new DateTime(1997, 9, 9), Description = "Photographer", PhotoPath = "avatars/dora.png", NeptunCode = "DORA01", Password = "pass5", IsActive = false },
                    new User { Name = "Gergely Tóth", BirthDate = new DateTime(1995, 4, 21), Description = "Backend Developer", PhotoPath = "avatars/gergely.png", NeptunCode = "GTO555", Password = "pass6", IsActive = true },
                    new User { Name = "Noémi Varga", BirthDate = new DateTime(2000, 12, 30), Description = "UX Researcher", PhotoPath = "avatars/noemi.png", NeptunCode = "NOV999", Password = "pass7", IsActive = true },
                    new User { Name = "Miklós Farkas", BirthDate = new DateTime(1998, 6, 2), Description = "DevOps", PhotoPath = "avatars/miklos.png", NeptunCode = "MFK321", Password = "pass8", IsActive = true },
                    new User { Name = "Lea Müller", BirthDate = new DateTime(1996, 3, 12), Description = "Exchange Student", PhotoPath = "avatars/lea.png", NeptunCode = "LEM444", Password = "pass9", IsActive = true },
                    new User { Name = "Carlos Silva", BirthDate = new DateTime(1994, 8, 27), Description = "Data Scientist", PhotoPath = "avatars/carlos.png", NeptunCode = "CSL202", Password = "pass10", IsActive = true }
                };

                // insert users (sqlite-net will populate AutoIncrement Id on the objects)
                foreach (var u in seedUsers)
                    await database.InsertAsync(u);

                // refresh to get assigned Ids
                users = await database.Table<User>().ToListAsync();

                // seed some RightSwipe entries if none exist
                var swipes = await database.Table<RightSwipe>().ToListAsync();
                if (swipes.Count == 0 && users.Count >= 4)
                {
                    // example: mutual match between users[0] and users[2]
                    var swipeA = new RightSwipe { LikerId = users[0].Id, LikedId = users[2].Id, IsMatch = true, CreatedAt = DateTime.UtcNow.AddDays(-7) };
                    var swipeB = new RightSwipe { LikerId = users[2].Id, LikedId = users[0].Id, IsMatch = true, CreatedAt = DateTime.UtcNow.AddDays(-6) };

                    // one-sided likes
                    var swipeC = new RightSwipe { LikerId = users[1].Id, LikedId = users[3].Id, IsMatch = false, CreatedAt = DateTime.UtcNow.AddDays(-3) };
                    var swipeD = new RightSwipe { LikerId = users[4].Id, LikedId = users[1].Id, IsMatch = false, CreatedAt = DateTime.UtcNow.AddDays(-2) };

                    // another mutual match (different pair)
                    var swipeE = new RightSwipe { LikerId = users[5].Id, LikedId = users[6].Id, IsMatch = true, CreatedAt = DateTime.UtcNow.AddDays(-10) };
                    var swipeF = new RightSwipe { LikerId = users[6].Id, LikedId = users[5].Id, IsMatch = true, CreatedAt = DateTime.UtcNow.AddDays(-9) };

                    await database.InsertAsync(swipeA);
                    await database.InsertAsync(swipeB);
                    await database.InsertAsync(swipeC);
                    await database.InsertAsync(swipeD);
                    await database.InsertAsync(swipeE);
                    await database.InsertAsync(swipeF);
                }
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
