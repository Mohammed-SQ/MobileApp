using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FMMSRestaurant.Models;
using FMMSRestaurant.Services;
using MenuItem = FMMSRestaurant.Models.MenuItemModel;

namespace FMMSRestaurant.ViewModels
{
    public partial class ManageMenuItemsViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public ManageMenuItemsViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        [ObservableProperty]
        private MenuItem[] _menuItems = [];

        [ObservableProperty]
        private MenuCategoryModel? _selectedCategory = null;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private MenuItemModel _menuItem = new();

        private bool _isInitialized;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            IsLoading = true;

            Categories = (await _apiService.GetMenuCategoriesAsync()).ToArray();

            if (Categories.Length > 0)
            {
                Categories[0].IsSelected = true;
                SelectedCategory = Categories[0];
                MenuItems = await _apiService.GetMenuItemsByCategoryIdAsync(SelectedCategory.Id);
            }

            SetEmptyCategoriesToItem();
            IsLoading = false;
        }

        [RelayCommand]
        private async Task SelectCategoryAsync(int categoryId)
        {
            if (SelectedCategory?.Id == categoryId)
                return;

            IsLoading = true;

            var currentSelectedCategory = Categories.First(c => c.IsSelected);
            currentSelectedCategory.IsSelected = false;

            var newSelectedCategory = Categories.First(c => c.Id == categoryId);
            newSelectedCategory.IsSelected = true;

            SelectedCategory = newSelectedCategory;
            MenuItems = await _apiService.GetMenuItemsByCategoryIdAsync(SelectedCategory.Id);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task EditMenuItemAsync(MenuItem menuItem)
        {
            var menuItemModel = new MenuItemModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Icon = menuItem.Icon
            };

            var itemCategories = await _apiService.GetCategoriesByMenuItemIdAsync(menuItem.Id);

            foreach (var category in Categories)
            {
                var categoryOfItem = new MenuCategoryModel
                {
                    Icon = category.Icon,
                    Id = category.Id,
                    Name = category.Name,
                    IsSelected = itemCategories.Any(c => c.Id == category.Id)
                };
                menuItemModel.Categories.Add(categoryOfItem);
            }

            MenuItem = menuItemModel;
        }

        private void SetEmptyCategoriesToItem()
        {
            MenuItem.Categories.Clear();
            foreach (var category in Categories)
            {
                var categoryOfItem = new MenuCategoryModel
                {
                    Id = category.Id,
                    Icon = category.Icon,
                    Name = category.Name,
                    IsSelected = false
                };
                MenuItem.Categories.Add(categoryOfItem);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            MenuItem = new();
            SetEmptyCategoriesToItem();
        }

        [RelayCommand]
        private async Task SaveMenuItemAsync(MenuItemModel model)
        {
            IsLoading = true;

            var errorMessage = await _apiService.SaveMenuItemAsync(model);
            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
            }
            else
            {
                await Toast.Make("Menu item saved successfully").Show();
                HandleMenuItemChanged(model);
                WeakReferenceMessenger.Default.Send(MenuItemChangedMessage.From(model));
                Cancel();
            }

            IsLoading = false;
        }

        private void HandleMenuItemChanged(MenuItemModel model)
        {
            var menuItem = MenuItems.FirstOrDefault(m => m.Id == model.Id);
            if (menuItem != null)
            {
                if (!model.SelectedCategories.Any(c => c.Id == SelectedCategory.Id))
                {
                    MenuItems = [.. MenuItems.Where(m => m.Id != model.Id)];
                    return;
                }

                menuItem.Name = model.Name;
                menuItem.Price = model.Price;
                menuItem.Description = model.Description;
                menuItem.Icon = model.Icon;

                MenuItems = [.. MenuItems];
            }
            else if (model.SelectedCategories.Any(c => c.Id == SelectedCategory.Id))
            {
                var newMenuItem = new MenuItem
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    Description = model.Description,
                    Icon = model.Icon
                };

                MenuItems = [.. MenuItems, newMenuItem];
            }
        }
    }
}
