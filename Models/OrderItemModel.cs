namespace FMMSRestaurant.Models
{
    public class OrderItemModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Amount => Price * Quantity; // Added for binding
    }
}