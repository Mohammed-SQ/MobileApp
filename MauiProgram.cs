using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using FMMSRestaurant.Pages;
using FMMSRestaurant.ViewModels;

namespace FMMSRestaurant
{
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
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services
                .AddSingleton<HomeViewModel>()
                .AddSingleton<MainPage>()
                .AddSingleton<OrdersPage>()
                .AddTransient<ManageMenuItemsViewModel>()
                .AddTransient<ManageMenuItemPage>()
                .AddSingleton<SettingsViewModel>();

            return builder.Build();
        }
    }
}
