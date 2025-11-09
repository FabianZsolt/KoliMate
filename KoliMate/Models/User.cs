using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace KoliMate.Models
{
    [ObservableObject]
    public partial class User
    {
        [property: PrimaryKey, AutoIncrement]
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private DateTime birthDate;

        partial void OnBirthDateChanged(DateTime value)
        {
            OnPropertyChanged(nameof(Age));
        }

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string photoPath = string.Empty; // lokalis fajlpath

        [ObservableProperty]
        private string neptunCode = string.Empty; // ha regisztracio Neptun kod alapjan

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isActive;

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
