using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FMMSRestaurant.Models;
using Microsoft.Data.SqlClient;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FMMSRestaurant.ViewModels;

public partial class OrdersViewModel : ObservableObject
{
    private readonly string _connectionString = "Data Source=SQL1003.site4now.net,1433;Initial Catalog=db_ab85c8_apprestaurant;User Id=db_ab85c8_apprestaurant_admin;Password=m1234567;TrustServerCertificate=True;";

    public OrdersViewModel()
    {
        SelectOrderCommand = new AsyncRelayCommand<OrderModel>(OnSelectOrder);
    }

    public ObservableCollection<OrderModel> Orders { get; set; } = new();
    public ObservableCollection<OrderItemModel> OrderItems { get; set; } = new();
    public bool IsLoading { get; set; }

    public async Task<bool> PlaceOrderAsync(List<CartModel> cartItems, bool isPaidCash)
    {
        var order = new OrderModel
        {
            OrderDate = DateTime.Now,
            PaymentMode = isPaidCash ? "Cash" : "Card",
            TotalAmountPaid = cartItems.Sum(c => c.Amount),
            TotalItemsCount = cartItems.Sum(c => c.Quantity),
            Items = cartItems.Select(c => new OrderItemModel
            {
                Id = c.ItemId,
                Name = c.Name ?? string.Empty,
                Icon = c.Icon ?? string.Empty,
                Price = c.Price,
                Quantity = c.Quantity
            }).ToArray()
        };

        var result = await SaveOrderToDatabaseAsync(order);
        if (result == null)
        {
            Orders.Add(order);
        }
        return result == null;
    }

    private async Task<string?> SaveOrderToDatabaseAsync(OrderModel order)
    {
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            string insertOrderQuery = @"INSERT INTO Orders (OrderDate, PaymentMode, TotalAmountPaid, TotalItemsCount)
                                        OUTPUT INSERTED.Id
                                        VALUES (@OrderDate, @PaymentMode, @TotalAmountPaid, @TotalItemsCount)";
            using SqlCommand orderCmd = new(insertOrderQuery, conn, transaction);
            orderCmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            orderCmd.Parameters.AddWithValue("@PaymentMode", order.PaymentMode ?? (object)DBNull.Value);
            orderCmd.Parameters.AddWithValue("@TotalAmountPaid", order.TotalAmountPaid);
            orderCmd.Parameters.AddWithValue("@TotalItemsCount", order.TotalItemsCount);

            int orderId = (int)(await orderCmd.ExecuteScalarAsync() ?? 0);
            order.Id = orderId;

            foreach (var item in order.Items ?? Array.Empty<OrderItemModel>())
            {
                string insertItemQuery = @"INSERT INTO OrderItems (OrderId, MenuItemId, Name, Icon, Price, Quantity)
                                           VALUES (@OrderId, @MenuItemId, @Name, @Icon, @Price, @Quantity)";
                using SqlCommand itemCmd = new(insertItemQuery, conn, transaction);
                itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                itemCmd.Parameters.AddWithValue("@MenuItemId", item.Id);
                itemCmd.Parameters.AddWithValue("@Name", item.Name ?? (object)DBNull.Value);
                itemCmd.Parameters.AddWithValue("@Icon", item.Icon ?? (object)DBNull.Value);
                itemCmd.Parameters.AddWithValue("@Price", item.Price);
                itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                await itemCmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return null;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return ex.Message;
        }
    }

    public async Task LoadOrdersAsync()
    {
        IsLoading = true;
        Orders.Clear();
        OrderItems.Clear();

        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = @"SELECT Id, OrderDate, PaymentMode, TotalAmountPaid, TotalItemsCount FROM Orders";
        using SqlCommand cmd = new(query, conn);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Orders.Add(new OrderModel
            {
                Id = reader.GetInt32(0),
                OrderDate = reader.GetDateTime(1),
                PaymentMode = reader.GetString(2),
                TotalAmountPaid = reader.GetDecimal(3),
                TotalItemsCount = reader.GetInt32(4),
                Items = Array.Empty<OrderItemModel>()
            });
        }

        IsLoading = false;
    }

    private OrderModel? _selectedOrder;

    public IAsyncRelayCommand<OrderModel> SelectOrderCommand { get; }

    private async Task OnSelectOrder(OrderModel? order)
    {
        if (order == null || order == _selectedOrder)
        {
            _selectedOrder = null;
            OrderItems.Clear();
            foreach (var o in Orders) o.IsSelected = false;
            return;
        }

        _selectedOrder = order;
        foreach (var o in Orders) o.IsSelected = o == order;

        OrderItems.Clear();
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = @"SELECT MenuItemId, Name, Icon, Price, Quantity FROM OrderItems WHERE OrderId = @OrderId";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@OrderId", order.Id);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            OrderItems.Add(new OrderItemModel
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Icon = reader.GetString(2),
                Price = reader.GetDecimal(3),
                Quantity = reader.GetInt32(4)
            });
        }
    }
}