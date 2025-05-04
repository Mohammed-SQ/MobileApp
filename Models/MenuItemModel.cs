namespace FMMSRestaurant.Models;

public class MenuItemModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Matches nvarchar(100)
    public string Name { get; set; } = string.Empty; // Non-nullable with default
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Icon { get; set; } // Maps to ImageUrl
    public string? Category { get; set; } // Maps to Category column
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<MenuCategoryModel>? SelectedCategories { get; set; }
}