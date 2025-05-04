using CommunityToolkit.Maui.Views;
using FMMSRestaurant.Controls;
using FMMSRestaurant.Pages;

namespace FMMSRestaurant;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes explicitly
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));
        Routing.RegisterRoute(nameof(ManageMenuItemPage), typeof(ManageMenuItemPage));
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var helpPopup = new HelpPopup();
        await this.ShowPopupAsync(helpPopup);
    }
}