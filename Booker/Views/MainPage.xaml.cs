using Booker.ViewModels;

namespace Booker.Views;

public partial class MainPage : ContentPage
{
    // ViewModel is injected by DI. The XAML no longer declares <vm:MainViewModel /> inline,
    // so the BindingContext is set here instead — the DI-provided singleton instance.
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
