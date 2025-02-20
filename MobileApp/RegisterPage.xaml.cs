namespace MobileApp
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            bool isSuccess = await DatabaseHelper.RegisterUser(username, email, password);

            if (isSuccess)
            {
                await DisplayAlert("Success", "Account created!", "OK");
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                await DisplayAlert("Error", "Registration failed.", "OK");
            }
        }
    }
}
