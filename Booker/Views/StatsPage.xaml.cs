using Booker.ViewModels;

namespace Booker.Views;

public partial class StatsPage : ContentPage
{
    public StatsPage(StatsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
