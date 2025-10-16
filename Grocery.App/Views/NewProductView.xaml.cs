using Grocery.Core.ViewModels;

namespace Grocery.App.Views;

public partial class NewProductView : ContentPage
{
    public NewProductView(NewProductViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}