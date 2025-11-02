using KoliMate.ViewModels;
using KoliMate.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KoliMate.Views
{
    public partial class ProfilePage : ContentPage
    {
        private ProfilePageViewModel VM;
      

        public ProfilePage(ProfilePageViewModel VM)
        {
            InitializeComponent();
            this.VM = VM;
            BindingContext = VM;
        }
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is ProfilePageViewModel vm)
                await vm.InitAsync();
        }

    }
}
