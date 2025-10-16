using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Models;
using Grocery.Core.Services;
using System.Windows.Input;
using Grocery.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;
using Grocery.App.ViewModels;

namespace Grocery.Core.ViewModels
{
    public partial class NewProductViewModel : ObservableObject
    {
        private readonly ProductService _productService;
        private readonly GlobalViewModel _userService;
        private readonly Action<Product>? _onProductAdded;

        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private int stock;
        [ObservableProperty] private DateOnly shelfLife = DateOnly.FromDateTime(DateTime.Today);
        [ObservableProperty] private decimal price;

        public ICommand AddProductCommand { get; }

        public NewProductViewModel(ProductService productService, GlobalViewModel userService, Action<Product>? onProductAdded = null)
        {
            _productService = productService;
            _userService = userService;
            _onProductAdded = onProductAdded;
            AddProductCommand = new RelayCommand(AddProduct);
        }

        private void AddProduct()
        {
            if (_userService.Client?.Role.ToString() != "admin")
            {
                return;
            }

            var product = new Product(0, name, stock, shelfLife, price);
            var added = _productService.Add(product);
            _onProductAdded?.Invoke(added);
        }
    }
}