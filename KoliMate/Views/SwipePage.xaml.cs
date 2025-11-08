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

        
    }
}
