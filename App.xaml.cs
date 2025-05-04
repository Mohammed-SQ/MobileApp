namespace FMMSRestaurant;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set the main page of the app to the Shell
        MainPage = new AppShell();
    }
}
