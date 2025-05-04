using FMMSRestaurant.Models;

namespace FMMSRestaurant.Services
{
    public class ApiService
    {
        private readonly CloudDatabaseService _db = new();

        public async Task<List<MenuCategoryModel>> GetMenuCategoriesAsync()
        {
            return await _db.GetMenuCategoriesAsync();
        }

        public async Task<MenuItemModel[]> GetMenuItemsByCategoryIdAsync(int categoryId)
        {
            return await _db.GetMenuItemsByCategoryIdAsync(categoryId);
        }

        public async Task<List<MenuCategoryModel>> GetCategoriesByMenuItemIdAsync(int menuItemId)
        {
            return await _db.GetCategoriesByMenuItemIdAsync(menuItemId);
        }

        public async Task<string?> SaveMenuItemAsync(MenuItemModel model)
        {
            return await _db.SaveMenuItemAsync(model);
        }
    }
}
