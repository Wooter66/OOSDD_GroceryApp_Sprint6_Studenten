using Grocery.App.ViewModels;
using Grocery.Core.Interfaces.Services;
using Microsoft.Maui.Controls;

namespace Grocery.App.Views;

public partial class ProductView : ContentPage
{
    private readonly GlobalViewModel _global;

    public ProductView(ProductViewModel viewModel, GlobalViewModel global)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _global = global;
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        if (_global.Client.Role == Core.Models.Role.Admin)
        {
            await Shell.Current.GoToAsync(nameof(NewProductView));
        }
        else
        {
            await DisplayAlert("Geen toegang", "Alleen admins mogen producten toevoegen.", "OK");
        }
    }
}