using FMMSRestaurant.Models;
using FMMSRestaurant.ViewModels;

namespace FMMSRestaurant.Pages;

public partial class ManageMenuItemPage : ContentPage
{
    private readonly ManageMenuItemsViewModel _viewModel;

    public ManageMenuItemPage(ManageMenuItemsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Initialize the view model asynchronously
        Loaded += async (s, e) => await _viewModel.InitializeAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Bind the Picker's SelectedIndexChanged to SelectCategoryCommand
        var categoryPicker = this.FindByName<Picker>("CategoryPicker");
        if (categoryPicker != null)
        {
            categoryPicker.SelectedIndexChanged += (s, e) =>
            {
                var selectedCategory = categoryPicker.SelectedItem as MenuCategoryModel;
                if (selectedCategory != null)
                {
                    _viewModel.SelectCategoryCommand.Execute(selectedCategory);
                }
            };
        }

        // Command bindings are already set in XAML, so these are redundant
        // If you need to set them programmatically, ensure they aren't duplicated in XAML
    }
}