using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System;

namespace KoliMate.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }     
        public int Age { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; } // lokalis fajlpath
        public string NeptunCode { get; set; } // ha regisztracio Neptun kod alapjan
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
