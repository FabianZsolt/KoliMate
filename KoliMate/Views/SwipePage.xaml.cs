namespace KoliMate.Views
{
    public partial class SwipePage : ContentPage
    {
        int count = 0;

        public SwipePage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Kattintások száma: {count}";
            else
                CounterBtn.Text = $"Kattintások száma: {count}";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
