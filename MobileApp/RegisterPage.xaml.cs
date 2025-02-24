using System.Data.SqlClient;
using Microsoft.Maui.Controls;

namespace MobileApp
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            string connectionString = "Data Source=tcp:sql6033.site4now.net,1433;Initial Catalog=db_ab32ed_aircraftdb;User ID=db_ab32ed_aircraftdb_admin;Password=m1234567;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO [dbo].[Users] (FullName, Email, Password, Role, PhoneNumber, Address, State, ZIP, CreatedAt) VALUES (@FullName, @Email, @Password, 'Customer', @PhoneNumber, @Address, @State, @ZIP, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", FullNameEntry.Text);
                        cmd.Parameters.AddWithValue("@Email", EmailEntry.Text);
                        cmd.Parameters.AddWithValue("@Password", PasswordEntry.Text);
                        cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumberEntry.Text);
                        cmd.Parameters.AddWithValue("@Address", AddressEntry.Text);
                        cmd.Parameters.AddWithValue("@State", StatePicker.SelectedItem?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@ZIP", ZIPEntry.Text);
                        cmd.ExecuteNonQuery();
                    }
                    await DisplayAlert("Success", "Registered successfully!", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (SqlException ex)
            {
                await DisplayAlert("Error", "Registration failed: " + ex.Message, "OK");
            }
        }

        private async void OnLoginHereClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

    }
}