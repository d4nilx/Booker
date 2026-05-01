namespace Booker.Views;

public partial class BookDetailPage : ContentPage
{
    public BookDetailPage(ViewModels.BookDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}