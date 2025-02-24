using System.Data.SqlClient;
using Microsoft.Maui.Controls;

namespace MobileApp
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text.Trim();
            string password = PasswordEntry.Text.Trim();

            string connectionString = "Data Source=tcp:sql6033.site4now.net,1433;Initial Catalog=db_ab32ed_aircraftdb;User ID=db_ab32ed_aircraftdb_admin;Password=m1234567;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM [dbo].[Users] WHERE Email = @Email AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader.GetString(1);
                                string role = reader.GetString(4);
                                await Navigation.PushAsync(new AircraftPage());
                            }
                            else
                            {
                                await DisplayAlert("Error", "Invalid email or password", "OK");
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    await DisplayAlert("Error", $"Database connection failed: {ex.Message}", "OK");
                }
            }
        }
    }
}