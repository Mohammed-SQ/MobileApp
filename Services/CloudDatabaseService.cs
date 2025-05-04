using System.Data;
using Microsoft.Data.SqlClient;
using FMMSRestaurant.Models;

namespace FMMSRestaurant.Services;

public class CloudDatabaseService
{
    private readonly string _connectionString = "Server=SQL1003.site4now.net,1433;Database=db_ab85c8_apprestaurant;User Id=db_ab85c8_apprestaurant_admin;Password=m1234567;TrustServerCertificate=True;";

    public async Task<List<MenuCategoryModel>> GetMenuCategoriesAsync()
    {
        List<MenuCategoryModel> categories = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            string query = "SELECT DISTINCT Category FROM MenuItems WHERE Category IS NOT NULL";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(new MenuCategoryModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    Icon = ""
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetMenuCategoriesAsync: {ex.Message}");
        }
        return categories;
    }

    public async Task<MenuItemModel[]> GetMenuItemsByCategoryIdAsync(string categoryId)
    {
        List<MenuItemModel> items = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            string query = @"
                SELECT Id, Name, Description, Price, ImageUrl
                FROM MenuItems
                WHERE Category = @Category";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Category", categoryId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new MenuItemModel
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Icon = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetMenuItemsByCategoryIdAsync: {ex.Message}");
        }
        return items.ToArray();
    }

    public async Task<List<MenuCategoryModel>> GetCategoriesByMenuItemIdAsync(string menuItemId)
    {
        List<MenuCategoryModel> categories = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            string query = @"
                SELECT Category
                FROM MenuItems
                WHERE Id = @MenuItemId";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(new MenuCategoryModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    Icon = ""
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCategoriesByMenuItemIdAsync: {ex.Message}");
        }
        return categories;
    }

    public async Task<string?> SaveMenuItemAsync(MenuItemModel model)
    {
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            SqlTransaction transaction = conn.BeginTransaction();

            SqlCommand cmd;
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
                string insertQuery = @"INSERT INTO MenuItems (Id, Name, Description, Price, ImageUrl, Category, IsAvailable, CreatedAt, UpdatedAt)
                                       VALUES (@Id, @Name, @Description, @Price, @ImageUrl, @Category, @IsAvailable, @CreatedAt, @UpdatedAt)";
                cmd = new(insertQuery, conn, transaction);
            }
            else
            {
                string updateQuery = @"UPDATE MenuItems
                                       SET Name = @Name, Description = @Description, Price = @Price, ImageUrl = @ImageUrl, Category = @Category, IsAvailable = @IsAvailable, UpdatedAt = @UpdatedAt
                                       WHERE Id = @Id";
                cmd = new(updateQuery, conn, transaction);
            }

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", model.Price);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)model.Icon ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object?)model.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAvailable", model.IsAvailable);
            cmd.Parameters.AddWithValue("@CreatedAt", model.CreatedAt);
            cmd.Parameters.AddWithValue("@UpdatedAt", model.UpdatedAt);

            await cmd.ExecuteNonQueryAsync();
            transaction.Commit();
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveMenuItemAsync: {ex.Message}");
            return ex.Message;
        }
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        List<OrderModel> orders = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            // Step 1: Fetch all orders
            string ordersQuery = @"
                SELECT OrderId, UserId, Status, Type, TableId, Total, CustomerNameSnapshot, CreatedAt, UpdatedAt
                FROM Orders
                ORDER BY CreatedAt DESC";
            using SqlCommand ordersCmd = new(ordersQuery, conn);
            using SqlDataReader ordersReader = await ordersCmd.ExecuteReaderAsync();

            Dictionary<string, OrderModel> orderDict = new();
            while (await ordersReader.ReadAsync())
            {
                var order = new OrderModel
                {
                    OrderId = ordersReader.GetString(0),
                    UserId = ordersReader.IsDBNull(1) ? null : ordersReader.GetInt32(1),
                    Status = ordersReader.GetString(2),
                    PaymentMode = ordersReader.GetString(3), // Map Type to PaymentMode
                    TableId = ordersReader.IsDBNull(4) ? null : ordersReader.GetString(4),
                    Total = ordersReader.GetDecimal(5),
                    CustomerNameSnapshot = ordersReader.IsDBNull(6) ? null : ordersReader.GetString(6),
                    CreatedAt = ordersReader.GetDateTime(7),
                    UpdatedAt = ordersReader.GetDateTime(8),
                    Items = new OrderItemModel[0] // Will populate later
                };
                orderDict[order.OrderId] = order;
                orders.Add(order);
            }
            ordersReader.Close();

            // Step 2: Fetch all order items for the orders
            string itemsQuery = @"
                SELECT oi.OrderId, oi.OrderItemId, oi.MenuItemId, oi.Quantity, oi.Price, oi.SpecialRequests, oi.CreatedAt, mi.Name, mi.ImageUrl
                FROM OrderItems oi
                LEFT JOIN MenuItems mi ON oi.MenuItemId = mi.Id
                WHERE oi.OrderId IN (SELECT OrderId FROM Orders)";
            using SqlCommand itemsCmd = new(itemsQuery, conn);
            using SqlDataReader itemsReader = await itemsCmd.ExecuteReaderAsync();

            while (await itemsReader.ReadAsync())
            {
                string orderId = itemsReader.GetString(0);
                if (orderDict.TryGetValue(orderId, out var order))
                {
                    var orderItem = new OrderItemModel
                    {
                        OrderItemId = itemsReader.GetInt32(1),
                        OrderId = orderId,
                        MenuItemId = itemsReader.GetInt32(2).ToString(),
                        Quantity = itemsReader.GetInt32(3),
                        Price = itemsReader.GetDecimal(4),
                        SpecialRequests = itemsReader.IsDBNull(5) ? null : itemsReader.GetString(5),
                        CreatedAt = itemsReader.GetDateTime(6),
                        Name = itemsReader.IsDBNull(7) ? null : itemsReader.GetString(7),
                        Icon = itemsReader.IsDBNull(8) ? null : itemsReader.GetString(8)
                    };
                    order.Items = order.Items.Concat(new[] { orderItem }).ToArray();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetOrdersAsync: {ex.Message}");
        }
        return orders;
    }
}