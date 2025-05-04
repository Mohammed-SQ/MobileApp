using CommunityToolkit.Mvvm.ComponentModel;

namespace FMMSRestaurant.Models;

public partial class OrderModel : ObservableObject
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string? PaymentMode { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public int TotalItemsCount { get; set; }
    public OrderItemModel[]? Items { get; set; }

    [ObservableProperty]
    private bool isSelected;
}