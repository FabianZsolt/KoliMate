using CommunityToolkit.Mvvm.ComponentModel;
using KoliMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoliMate.ViewModels
{
    public class ProfilePageViewModel : ObservableObject
    {
        IDatabaseService db;

        public ProfilePageViewModel(IDatabaseService db)
        {
            this.db = db;
        }

    }
}
