using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using FMMSRestaurant.Services;
using FMMSRestaurant.ViewModels;
using CommunityToolkit.Maui;

namespace FMMSRestaurant;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<CloudDatabaseService>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<OrdersViewModel>();
        builder.Services.AddTransient<ManageMenuItemsViewModel>();

        return builder.Build();
    }
}