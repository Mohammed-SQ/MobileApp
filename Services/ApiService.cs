using FMMSRestaurant.Models;
using FMMSRestaurant.Services;

namespace FMMSRestaurant.Services;

public class ApiService
{
    private readonly CloudDatabaseService _db = new();

    public async Task<List<MenuCategoryModel>> GetMenuCategoriesAsync()
    {
        return await _db.GetMenuCategoriesAsync() ?? new List<MenuCategoryModel>();
    }

    public async Task<MenuItemModel[]> GetMenuItemsByCategoryIdAsync(int categoryId)
    {
        return await _db.GetMenuItemsByCategoryIdAsync(categoryId) ?? Array.Empty<MenuItemModel>();
    }

    public async Task<List<MenuCategoryModel>> GetCategoriesByMenuItemIdAsync(int menuItemId)
    {
        return await _db.GetCategoriesByMenuItemIdAsync(menuItemId) ?? new List<MenuCategoryModel>();
    }

    public async Task<string?> SaveMenuItemAsync(MenuItemModel model)
    {
        if (model == null) return "Menu item cannot be null.";
        return await _db.SaveMenuItemAsync(model);
    }
}