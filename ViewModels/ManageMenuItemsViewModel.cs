using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMMSRestaurant.Models;
using FMMSRestaurant.Services;

namespace FMMSRestaurant.ViewModels;

public partial class ManageMenuItemsViewModel : ObservableObject
{
    private readonly CloudDatabaseService _dbService;

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

    private MenuCategoryModel? _selectedCategory;
    public MenuCategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    private MenuItemModel? _selectedItem;
    public MenuItemModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public ICommand AddCommand { get; }
    public ICommand EditMenuItemCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand SaveMenuItemCommand { get; }
    public ICommand CancelCommand { get; }

    public ManageMenuItemsViewModel(CloudDatabaseService dbService)
    {
        _dbService = dbService;
        AddCommand = new RelayCommand(AddItem);
        EditMenuItemCommand = new RelayCommand(EditItem, CanEdit);
        DeleteCommand = new RelayCommand(DeleteItem, CanDelete);
        SelectCategoryCommand = new AsyncRelayCommand<MenuCategoryModel>(SelectCategoryAsync);
        SaveMenuItemCommand = new AsyncRelayCommand(SaveMenuItemAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    public async Task InitializeAsync()
    {
        await LoadCategoriesAsync();
        await LoadMenuItemsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _dbService.GetMenuCategoriesAsync();
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
        SelectedCategory = Categories.FirstOrDefault();
    }

    private async Task LoadMenuItemsAsync()
    {
        if (SelectedCategory == null) return;

        var items = await _dbService.GetMenuItemsByCategoryIdAsync(SelectedCategory.Id);
        var allCategories = await _dbService.GetMenuCategoriesAsync();

        MenuItems.Clear();
        foreach (var item in items)
        {
            // Ensure SelectedCategories is not null
            if (item.SelectedCategories != null)
            {
                item.SelectedCategories = allCategories.Where(c => item.SelectedCategories.Any(sc => sc.Id == c.Id)).ToList();
            }
            else
            {
                item.SelectedCategories = new List<MenuCategoryModel>();
            }
            MenuItems.Add(item);
        }
    }

    private void AddItem()
    {
        var newItem = new MenuItemModel();
        MenuItems.Add(newItem);
        SelectedItem = newItem;
    }

    private bool CanEdit() => SelectedItem != null;

    private void EditItem()
    {
        if (SelectedItem != null)
        {
            // Open edit form or set up editing UI
            // Example: Navigate to SaveMenuItemFormControl with SelectedItem
        }
    }

    private bool CanDelete() => SelectedItem != null;

    private void DeleteItem()
    {
        if (SelectedItem != null)
        {
            MenuItems.Remove(SelectedItem);
            SelectedItem = null;
        }
    }

    private async Task SelectCategoryAsync(MenuCategoryModel? category)
    {
        if (category != null)
        {
            SelectedCategory = category;
            await LoadMenuItemsAsync();
        }
    }

    private bool CanSave() => SelectedItem != null;

    private async Task SaveMenuItemAsync()
    {
        if (SelectedItem != null && ValidateMenuItem(SelectedItem))
        {
            try
            {
                var result = await _dbService.SaveMenuItemAsync(SelectedItem);
                if (result != null)
                {
                    if (App.Current?.MainPage != null)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", result, "OK");
                    }
                }
                else
                {
                    await LoadMenuItemsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving menu item: {ex.Message}");
                if (App.Current?.MainPage != null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred.", "OK");
                }
            }
        }
        else
        {
            if (App.Current?.MainPage != null)
            {
                await App.Current.MainPage.DisplayAlert("Validation Error", "Please fill in all required fields.", "OK");
            }
        }
    }

    private bool ValidateMenuItem(MenuItemModel item)
    {
        return !string.IsNullOrWhiteSpace(item.Name) && item.Price > 0;
    }

    private void Cancel()
    {
        SelectedItem = null;
    }
}