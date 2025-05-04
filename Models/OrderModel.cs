using CommunityToolkit.Mvvm.ComponentModel;

namespace FMMSRestaurant.Models;

public class OrderModel
{
    public string OrderId { get; set; } = Guid.NewGuid().ToString();
    public int? UserId { get; set; }
    public string Status { get; set; } = "Pending";
    public string PaymentMode { get; set; } = "Cash";
    public string? TableId { get; set; }
    public decimal Total { get; set; }
    public string? CustomerNameSnapshot { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public OrderItemModel[] Items { get; set; } = Array.Empty<OrderItemModel>();
    public bool IsSelected { get; set; }
    public int TotalItemsCount => Items?.Length ?? 0; // Added calculated property
}
