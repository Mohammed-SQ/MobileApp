using FMMSRestaurant.Pages;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using FMMSRestaurant.ViewModels;

namespace FMMSRestaurant;

public partial class App : Application
{
    private readonly SettingsViewModel _settingsViewModel;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Resolve dependencies
        var homeViewModel = serviceProvider.GetRequiredService<HomeViewModel>();
        var settingsViewModel = serviceProvider.GetRequiredService<SettingsViewModel>();
        _settingsViewModel = settingsViewModel;

        MainPage = new MainPage(homeViewModel, settingsViewModel);

        // Initialize SettingsViewModel after MainPage is set
        _ = Task.Run(async () => await _settingsViewModel.InitializeAsync());
    }
}