using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System;

namespace KoliMate.App.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }     // pl. "Papp Gabor"
        public int Age { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; } // lokalis fajlpath
        public string NeptunHash { get; set; } // ha regisztracio Neptun kod alapjan
    }
}
