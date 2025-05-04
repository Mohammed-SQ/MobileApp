using FMMSRestaurant.Models;

namespace FMMSRestaurant.Controls;

public partial class CategoriesListControl : HorizontalStackLayout
{
    public static readonly BindableProperty CategoriesProperty =
        BindableProperty.Create(nameof(Categories), typeof(IEnumerable<MenuCategoryModel>), typeof(CategoriesListControl), null);

    public IEnumerable<MenuCategoryModel> Categories
    {
        get => (IEnumerable<MenuCategoryModel>)GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public event EventHandler<MenuCategoryModel>? OnCategorySelected;

    public CategoriesListControl()
    {
        InitializeComponent();
    }

    private void OnCategoryTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is MenuCategoryModel category)
        {
            OnCategorySelected?.Invoke(this, category);
        }
    }
}