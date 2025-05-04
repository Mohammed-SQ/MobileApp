using System.Data;
using Microsoft.Data.SqlClient;
using FMMSRestaurant.Models;

namespace FMMSRestaurant.Services;

public class CloudDatabaseService
{
    private readonly string _connectionString = "Server=SQL1003.site4now.net,1433;Database=db_ab85c8_apprestaurant;User Id=db_ab85c8_apprestaurant_admin;Password=m1234567;TrustServerCertificate=True;";

    public CloudDatabaseService()
    {
        // Test the connection during construction
        TestConnection();
    }

    private void TestConnection()
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            conn.Close();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Database connection failed: " + ex.Message, ex);
        }
    }

    public async Task<List<MenuCategoryModel>> GetMenuCategoriesAsync()
    {
        List<MenuCategoryModel> categories = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            string query = "SELECT Id, Name, Icon FROM MenuCategories";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(new MenuCategoryModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Icon = reader.GetString(2)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetMenuCategoriesAsync: {ex.Message}");
        }
        return categories;
    }

    public async Task<MenuItemModel[]> GetMenuItemsByCategoryIdAsync(int categoryId)
    {
        List<MenuItemModel> items = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            string query = @"
                SELECT m.Id, m.Name, m.Description, m.Price, m.Icon
                FROM MenuItems m
                JOIN MenuItemCategories mic ON m.Id = mic.MenuItemId
                WHERE mic.CategoryId = @CategoryId";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new MenuItemModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Icon = reader.GetString(4)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetMenuItemsByCategoryIdAsync: {ex.Message}");
        }
        return items.ToArray();
    }

    public async Task<List<MenuCategoryModel>> GetCategoriesByMenuItemIdAsync(int menuItemId)
    {
        List<MenuCategoryModel> categories = new();
        try
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            string query = @"
                SELECT c.Id, c.Name, c.Icon
                FROM MenuCategories c
                JOIN MenuItemCategories mic ON c.Id = mic.CategoryId
                WHERE mic.MenuItemId = @MenuItemId";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(new MenuCategoryModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Icon = reader.GetString(2)
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
            if (model.Id == 0)
            {
                string insertQuery = @"INSERT INTO MenuItems (Name, Description, Price, Icon)
                                      OUTPUT INSERTED.Id
                                      VALUES (@Name, @Description, @Price, @Icon)";
                cmd = new(insertQuery, conn, transaction);
            }
            else
            {
                string updateQuery = @"UPDATE MenuItems
                                      SET Name = @Name, Description = @Description, Price = @Price, Icon = @Icon
                                      WHERE Id = @Id";
                cmd = new(updateQuery, conn, transaction);
                cmd.Parameters.AddWithValue("@Id", model.Id);
            }

            cmd.Parameters.AddWithValue("@Name", (object?)model.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", model.Price);
            cmd.Parameters.AddWithValue("@Icon", (object?)model.Icon ?? DBNull.Value);

            int menuItemId = model.Id == 0 ? (int)(await cmd.ExecuteScalarAsync() ?? 0) : model.Id;

            var clearCmd = new SqlCommand("DELETE FROM MenuItemCategories WHERE MenuItemId = @MenuItemId", conn, transaction);
            clearCmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
            await clearCmd.ExecuteNonQueryAsync();

            foreach (var category in model.SelectedCategories ?? new List<MenuCategoryModel>())
            {
                var catCmd = new SqlCommand("INSERT INTO MenuItemCategories (MenuItemId, CategoryId) VALUES (@MenuItemId, @CategoryId)", conn, transaction);
                catCmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                catCmd.Parameters.AddWithValue("@CategoryId", category.Id);
                await catCmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveMenuItemAsync: {ex.Message}");
            return ex.Message;
        }
    }
}