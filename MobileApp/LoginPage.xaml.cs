using System;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;

namespace MobileApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=AircraftDB;Integrated Security=True;";

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            string email = txtEmail.Text?.Trim();
            string password = txtPassword.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Email and password are required.", "OK");
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Password = @Password";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 1)
                        {
                            await DisplayAlert("Success", "Login successful!", "OK");
                            await Navigation.PushAsync(new MainPage());
                        }
                        else
                        {
                            await DisplayAlert("Error", "Invalid email or password.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Database connection failed: {ex.Message}", "OK");
            }
        }

        private async void GoToRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
