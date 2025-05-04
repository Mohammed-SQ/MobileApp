using Microsoft.Maui.Controls;
using FMMSRestaurant.ViewModels;

namespace FMMSRestaurant;

public partial class App : Application
{
    private readonly SettingsViewModel _settingsViewModel;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Resolve dependencies
        var settingsViewModel = serviceProvider.GetRequiredService<SettingsViewModel>();
        _settingsViewModel = settingsViewModel;

        // Initialize SettingsViewModel synchronously before setting MainPage
        Task.Run(async () => await _settingsViewModel.InitializeAsync()).GetAwaiter().GetResult();

        MainPage = new AppShell();
    }
}