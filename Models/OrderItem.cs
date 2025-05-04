namespace FMMSRestaurant.Models
{
    public class OrderItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal Amount => Price * Quantity;
    }
}
