using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System;

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

