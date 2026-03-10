using Booker.ViewModels;

namespace Booker.Views;

public partial class LibraryPage : ContentPage
{
    private readonly LibraryViewModel _viewModel;

    public LibraryPage(LibraryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // Reload data every time the user navigates back to this tab,
    // so books saved from SearchPage appear immediately without a manual pull-to-refresh.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.RefreshCommand.ExecuteAsync(null);
    }
}
