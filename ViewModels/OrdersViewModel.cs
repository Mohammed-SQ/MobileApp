using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMMSRestaurant.Models;
using FMMSRestaurant.Services;

namespace FMMSRestaurant.ViewModels;

public partial class OrdersViewModel : ObservableObject
{
    private readonly CloudDatabaseService _dbService;

    [ObservableProperty]
    private ObservableCollection<OrderModel> _orders = new();

    [ObservableProperty]
    private ObservableCollection<OrderItemModel> _orderItems = new();

    [ObservableProperty]
    private OrderModel? _selectedOrder;

    [ObservableProperty]
    private bool _isLoading;

    public ICommand LoadOrdersCommand { get; }
    public ICommand SelectOrderCommand { get; }
    public ICommand RefreshCommand { get; }

    public OrdersViewModel(CloudDatabaseService dbService)
    {
        _dbService = dbService;
        LoadOrdersCommand = new AsyncRelayCommand(LoadOrdersAsync);
        SelectOrderCommand = new RelayCommand<OrderModel>(SelectOrder);
        RefreshCommand = new AsyncRelayCommand(LoadOrdersAsync);
    }

    public async Task InitializeViewModelAsync()
    {
        await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        IsLoading = true;
        try
        {
            var orders = await _dbService.GetOrdersAsync() ?? new List<OrderModel>();
            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
            SelectedOrder = Orders.FirstOrDefault();
            if (SelectedOrder != null)
            {
                OrderItems.Clear();
                foreach (var item in SelectedOrder.Items)
                {
                    OrderItems.Add(item);
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SelectOrder(OrderModel? order)
    {
        if (order == null) return;
        SelectedOrder = order;
        OrderItems.Clear();
        foreach (var item in order.Items)
        {
            OrderItems.Add(item);
        }
    }

    public async Task<bool> PlaceOrderAsync(List<CartModel> cartItems, bool isPaidCash)
    {
        return await Task.FromResult(true);
    }
}