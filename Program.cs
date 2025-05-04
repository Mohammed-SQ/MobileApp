using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FMMSRestaurant.ViewModels;
using CommunityToolkit.Maui;
using FMMSRestaurant.Services; // Ensure this namespace is included

namespace FMMSRestaurant;

public class Program
{
    public static void Main(string[] args)
    {
        CreateMauiApp(); // No need to call Run() here
    }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit();

        // Register all required services
        builder.Services.AddTransient<CloudDatabaseService>();
        builder.Services.AddTransient<OrdersViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<HomeViewModel>();

        return builder.Build();
    }
}