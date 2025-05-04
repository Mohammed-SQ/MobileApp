using FMMSRestaurant.Models;
using FMMSRestaurant.ViewModels;
using Microsoft.Maui.Controls;

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
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (_settingsViewModel != null)
        {
            await _settingsViewModel.InitializeAsync();
        }
    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MenuItemModel menuItem)
        {
            _homeViewModel.AddToCartCommand.Execute(menuItem);
        }
        if (sender is CollectionView collectionView)
        {
            collectionView.SelectedItem = null;
        }
    }

    private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MenuCategoryModel category)
        {
            _homeViewModel.SelectCategoryCommand?.Execute(category);
        }
        if (sender is CollectionView collectionView)
        {
            collectionView.SelectedItem = null;
        }
    }
}