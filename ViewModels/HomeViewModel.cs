using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FMMSRestaurant.Models;
using FMMSRestaurant.Services;

namespace FMMSRestaurant.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly CloudDatabaseService _dbService;
    private readonly OrdersViewModel _ordersViewModel;

    private ObservableCollection<MenuItemModel> _menuItems = new();
    public ObservableCollection<MenuItemModel> MenuItems
    {
        get => _menuItems;
        set => SetProperty(ref _menuItems, value);
    }

    private ObservableCollection<MenuCategoryModel> _categories = new();
    public ObservableCollection<MenuCategoryModel> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    private ObservableCollection<CartModel> _cartItems = new();
    public ObservableCollection<CartModel> CartItems
    {
        get => _cartItems;
        set => SetProperty(ref _cartItems, value);
    }

    private MenuCategoryModel? _selectedCategory;
    public MenuCategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    private string _name = "Guest";
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private decimal _subtotal;
    public decimal Subtotal
    {
        get => _subtotal;
        set => SetProperty(ref _subtotal, value);
    }

    private int _taxPercentage;
    public int TaxPercentage
    {
        get => _taxPercentage;
        set
        {
            SetProperty(ref _taxPercentage, value);
            UpdateTaxAndTotal();
        }
    }

    private decimal _taxAmount;
    public decimal TaxAmount
    {
        get => _taxAmount;
        set => SetProperty(ref _taxAmount, value);
    }

    private decimal _total;
    public decimal Total
    {
        get => _total;
        set => SetProperty(ref _total, value);
    }

    private DateTime _currentDateTime = DateTime.Now;
    public DateTime CurrentDateTime
    {
        get => _currentDateTime;
        set => SetProperty(ref _currentDateTime, value);
    }

    public ICommand LoadItemsCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand AddToCartCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand RemoveItemFromCartCommand { get; }
    public ICommand ClearCartCommand { get; }
    public ICommand PlaceOrderCommand { get; }
    public ICommand TaxPercentageClickCommand { get; }

    public HomeViewModel(CloudDatabaseService dbService, OrdersViewModel ordersViewModel, SettingsViewModel settingsViewModel)
    {
        _dbService = dbService;
        _ordersViewModel = ordersViewModel;

        LoadItemsCommand = new RelayCommand(async () => await LoadItemsAsync());
        SelectCategoryCommand = new RelayCommand<MenuCategoryModel?>(SelectCategory);
        AddToCartCommand = new RelayCommand<MenuItemModel?>(AddToCart);
        DecreaseQuantityCommand = new RelayCommand<CartModel>(DecreaseQuantity);
        IncreaseQuantityCommand = new RelayCommand<CartModel>(IncreaseQuantity);
        RemoveItemFromCartCommand = new RelayCommand<CartModel>(RemoveItemFromCart);
        ClearCartCommand = new RelayCommand(ClearCart);
        PlaceOrderCommand = new RelayCommand<bool>(async paidCash => await PlaceOrderAsync(paidCash));
        TaxPercentageClickCommand = new RelayCommand(async () => await UpdateTaxPercentageAsync(settingsViewModel));

        WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, (recipient, message) =>
        {
            Name = message.Name;
        });

        TaxPercentage = settingsViewModel.GetTaxPercentage();
    }

    public async Task InitializeAsync()
    {
        await LoadItemsAsync();
    }

    private async Task LoadItemsAsync()
    {
        IsLoading = true;
        try
        {
            var categories = await _dbService.GetMenuCategoriesAsync();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            if (Categories.Any())
            {
                SelectedCategory = Categories.First();
                await LoadMenuItemsAsync();
            }
            else
            {
                if (App.Current?.MainPage != null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Failed to load menu categories. Please check your connection.", "OK");
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadMenuItemsAsync()
    {
        if (SelectedCategory == null) return;
        IsLoading = true;
        try
        {
            var items = await _dbService.GetMenuItemsByCategoryIdAsync(SelectedCategory.Id);
            MenuItems.Clear();
            foreach (var item in items)
            {
                item.SelectedCategories = await _dbService.GetCategoriesByMenuItemIdAsync(item.Id);
                MenuItems.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SelectCategory(MenuCategoryModel? category)
    {
        if (category == null) return;
        SelectedCategory = category;
        Task.Run(LoadMenuItemsAsync);
    }

    private void AddToCart(MenuItemModel? item)
    {
        if (item == null) return;
        var cartItem = CartItems.FirstOrDefault(c => c.ItemId == int.Parse(item.Id)); // Parse Id to int
        if (cartItem != null)
        {
            cartItem.Quantity++;
        }
        else
        {
            cartItem = new CartModel
            {
                ItemId = int.Parse(item.Id), // Parse Id to int
                Name = item.Name ?? string.Empty,
                Icon = item.Icon ?? string.Empty,
                Price = item.Price,
                Quantity = 1
            };
            CartItems.Add(cartItem);
        }
        UpdateCartTotals();
    }

    private void DecreaseQuantity(CartModel? item)
    {
        if (item == null) return;
        if (item.Quantity > 1)
        {
            item.Quantity--;
            UpdateCartTotals();
        }
        else
        {
            CartItems.Remove(item);
            UpdateCartTotals();
        }
    }

    private void IncreaseQuantity(CartModel? item)
    {
        if (item == null) return;
        item.Quantity++;
        UpdateCartTotals();
    }

    private void RemoveItemFromCart(CartModel? item)
    {
        if (item == null) return;
        CartItems.Remove(item);
        UpdateCartTotals();
    }

    private void ClearCart()
    {
        CartItems.Clear();
        UpdateCartTotals();
    }

    private async Task PlaceOrderAsync(bool isPaidCash)
    {
        if (!CartItems.Any()) return;

        IsLoading = true;
        try
        {
            var success = await _ordersViewModel.PlaceOrderAsync(CartItems.ToList(), isPaidCash);
            if (success)
            {
                CartItems.Clear();
                UpdateCartTotals();
                if (App.Current?.MainPage != null)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Order placed successfully!", "OK");
                }
            }
            else
            {
                if (App.Current?.MainPage != null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Failed to place order.", "OK");
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UpdateTaxPercentageAsync(SettingsViewModel settingsViewModel)
    {
        if (App.Current?.MainPage == null) return;

        var newTax = await App.Current.MainPage.DisplayPromptAsync("Tax Percentage", "Enter new tax percentage:", "OK", "Cancel", TaxPercentage.ToString(), keyboard: Keyboard.Numeric);
        if (int.TryParse(newTax, out int taxPercentage) && taxPercentage >= 0)
        {
            settingsViewModel.SetTaxPercentage(taxPercentage);
            TaxPercentage = taxPercentage;
        }
    }

    private void UpdateCartTotals()
    {
        Subtotal = CartItems.Sum(c => c.Amount);
        UpdateTaxAndTotal();
    }

    private void UpdateTaxAndTotal()
    {
        TaxAmount = Subtotal * (TaxPercentage / 100m);
        Total = Subtotal + TaxAmount;
    }
}