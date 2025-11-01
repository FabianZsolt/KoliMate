using KoliMate.ViewModels;
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
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
        }
    }
}
