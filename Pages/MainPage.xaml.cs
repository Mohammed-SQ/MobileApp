using FMMSRestaurant.Models;
using FMMSRestaurant.ViewModels;

namespace FMMSRestaurant.Pages;

public partial class MainPage : ContentPage
{
    private readonly HomeViewModel _homeViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    public MainPage(HomeViewModel homeViewModel, SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        _homeViewModel = homeViewModel;
        _settingsViewModel = settingsViewModel;
        BindingContext = _homeViewModel;
    }

            Initialize();
        }

        private async void Initialize()
        {
            await _homeViewModel.InitializeAsync();
        }

        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (_settingsViewModel != null)
            {
                await _settingsViewModel.InitializeAsync();
            }
        }

    private void OnItemSelected(object sender, MenuItemModel menuItem)
    {
        if (menuItem != null)
        {
            _homeViewModel.AddToCartCommand.Execute(menuItem); // Add item to cart
        }
    }
}