namespace FMMSRestaurant.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string? PaymentMode { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public int TotalItemsCount { get; set; }
        public OrderItemModel[] Items { get; set; } = Array.Empty<OrderItemModel>();
        public bool IsSelected { get; set; }
    }
}