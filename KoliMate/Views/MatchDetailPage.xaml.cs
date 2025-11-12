using KoliMate.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace KoliMate.Views
{
    [QueryProperty("UserId", "userId")]
    public partial class MatchDetailPage : ContentPage
    {
        private readonly MatchDetailViewModel viewModel;

        public MatchDetailPage() : this(MauiProgram.Services.GetRequiredService<MatchDetailViewModel>())
        {
        }

        public MatchDetailPage(MatchDetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = viewModel = vm;
        }

        // this property is set via Shell routing query
        public string UserId
        {
            set
            {
                if (int.TryParse(value, out var id))
                {
                    _ = viewModel.LoadUserAsync(id);
                }
            }
        }
    }
}