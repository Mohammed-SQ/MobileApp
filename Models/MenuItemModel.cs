using System.Collections.ObjectModel;

namespace FMMSRestaurant.Models
{
    public class MenuItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Icon { get; set; } = string.Empty;

        public ObservableCollection<MenuCategoryModel> Categories { get; set; } = new();

        public IEnumerable<MenuCategoryModel> SelectedCategories => Categories.Where(c => c.IsSelected);
    }
}
