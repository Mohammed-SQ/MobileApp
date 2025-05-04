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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _homeViewModel.InitializeAsync();
        await _settingsViewModel.InitializeAsync();
    }

    private void OnCategorySelected(object sender, MenuCategoryModel category)
    {
        if (category != null)
        {
            _homeViewModel.SelectCategoryCommand.Execute(category);
        }
    }

    private void OnItemSelected(object sender, MenuItemModel menuItem)
    {
        if (menuItem != null)
        {
            _homeViewModel.AddToCartCommand.Execute(menuItem);
        }
    }
}