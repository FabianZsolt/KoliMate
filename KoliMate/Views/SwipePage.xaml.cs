using KoliMate.ViewModels;

namespace KoliMate.Views
{
    public partial class SwipePage : ContentPage
    {
        private SwipePageViewModel VM;
        public SwipePage(SwipePageViewModel vm)
        {
            InitializeComponent();
            VM = vm;
            BindingContext = VM;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (VM.LoadUsersCommand is not null)
                await VM.LoadUsersCommand.ExecuteAsync(null);
        }
    }
}
