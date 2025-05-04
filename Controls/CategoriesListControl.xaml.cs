using System.Windows.Input;
using FMMSRestaurant.Models;

namespace FMMSRestaurant.Controls
{
    public partial class CategoriesListControl : ContentView
    {
        public CategoriesListControl()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CategoriesProperty = BindableProperty.Create(
            nameof(Categories),
            typeof(MenuCategoryModel[]),
            typeof(CategoriesListControl),
            Array.Empty<MenuCategoryModel>()
        );

        public MenuCategoryModel[] Categories
        {
            get => (MenuCategoryModel[])GetValue(CategoriesProperty);
            set => SetValue(CategoriesProperty, value);
        }

        public event Action<MenuCategoryModel>? OnCategorySelected;

        public ICommand SelectCategoryCommand => new Command<object>(parameter =>
        {
            if (parameter is MenuCategoryModel category)
            {
                OnCategorySelected?.Invoke(category);
            }
        });
    }
}
