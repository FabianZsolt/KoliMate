using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace KoliMate.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }     
        public DateTime BirthDate { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; } // lokalis fajlpath
        public string NeptunCode { get; set; } // ha regisztracio Neptun kod alapjan
        public string Password { get; set; }
        public bool IsActive { get; set; }

        // Computed age based on BirthDate. Ignored by SQLite so it is not persisted.
        [Ignore]
        public int Age
        {
            get
            {
                if (BirthDate == default) return 0;
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age < 0 ? 0 : age;
            }
        }
    }
}
