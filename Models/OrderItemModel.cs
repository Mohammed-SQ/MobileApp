namespace FMMSRestaurant.Models;

public class OrderItemModel
{
    public int OrderItemId { get; set; }
    public string? OrderId { get; set; } // Made nullable
    public string? MenuItemId { get; set; } // Made nullable (though schema suggests int, aligning with OrderId type for consistency)
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Icon { get; set; } // For UI
    public string? Name { get; set; } // For UI, made nullable
    public decimal Amount => Quantity * Price; // Calculated property
}