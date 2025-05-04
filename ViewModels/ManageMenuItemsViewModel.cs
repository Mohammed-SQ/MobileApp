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
        SelectCategoryCommand = new RelayCommand<MenuCategoryModel>(SelectCategory);
        SaveMenuItemCommand = new RelayCommand(SaveMenuItemAsync, CanSave);
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
        MenuItems.Clear();
        foreach (var item in items)
        {
            item.SelectedCategories = await _dbService.GetCategoriesByMenuItemIdAsync(item.Id);
            MenuItems.Add(item);
        }
    }

    private void AddItem()
    {
        var newItem = new MenuItemModel();
        MenuItems.Add(newItem);
        SelectedItem = newItem;
    }

    private bool CanEdit()
    {
        return SelectedItem != null;
    }

    private void EditItem()
    {
        if (SelectedItem != null)
        {
            // Edit logic here
        }
    }

    private bool CanDelete()
    {
        return SelectedItem != null;
    }

    private void DeleteItem()
    {
        if (SelectedItem != null)
        {
            MenuItems.Remove(SelectedItem);
            SelectedItem = null;
        }
    }

    private void SelectCategory(MenuCategoryModel? category)
    {
        if (category != null)
        {
            SelectedCategory = category;
            Task.Run(LoadMenuItemsAsync);
        }
    }

    private bool CanSave()
    {
        return SelectedItem != null;
    }

    private async void SaveMenuItemAsync()
    {
        if (SelectedItem != null)
        {
            var result = await _dbService.SaveMenuItemAsync(SelectedItem);
            if (result != null)
            {
                // Handle error with null check
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
    }

    private void Cancel()
    {
        SelectedItem = null;
    }
}