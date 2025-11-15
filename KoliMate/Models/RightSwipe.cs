using System;
using SQLite;

namespace KoliMate.Models
{
    public class RightSwipe
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int LikerId { get; set; }
        public int LikedId { get; set; }
        public bool IsMatch { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

