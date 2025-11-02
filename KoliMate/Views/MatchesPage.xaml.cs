using KoliMate.ViewModels;
using KoliMate.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KoliMate.Views
{
    public partial class MatchesPage : ContentPage
    {
        private MatchesPageViewModel VM;


        // Default constructor required for XAML previewer / Shell instantiation
        public MatchesPage() : this(App.Current?.Handler?.MauiContext?.Services?.GetService<MatchesPageViewModel>())
        {
        }


        public MatchesPage(MatchesPageViewModel VM)
        {
            InitializeComponent();
            this.VM = VM;
            BindingContext = VM;
        }
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is MatchesPageViewModel vm)
                await vm.InitAsync();
        }

    }
}
