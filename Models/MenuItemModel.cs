namespace FMMSRestaurant.Models;

public class MenuItemModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<MenuCategoryModel> SelectedCategories { get; set; } = new List<MenuCategoryModel>();
}