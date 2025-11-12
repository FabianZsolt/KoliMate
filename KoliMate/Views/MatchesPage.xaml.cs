using Microsoft.Extensions.DependencyInjection;
using KoliMate.ViewModels;
using KoliMate.Models;

namespace KoliMate.Views;

public partial class MatchesPage : ContentPage
{
    private readonly MatchesPageViewModel viewModel;

    // Parameterless constructor for XAML/DOTNET to instantiate the page
    // It resolves the view model from the application's service provider.
    public MatchesPage() : this(MauiProgram.Services.GetRequiredService<MatchesPageViewModel>())
    {
    }

    public MatchesPage(MatchesPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadMatchesCommand.ExecuteAsync(null);
    }
}
