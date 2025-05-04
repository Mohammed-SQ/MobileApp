namespace FMMSRestaurant.Models;

public class OrderItemModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Amount => Price * Quantity;
}