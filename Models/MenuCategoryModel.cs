using CommunityToolkit.Mvvm.ComponentModel;

namespace FMMSRestaurant.Models
{
    public partial class MenuCategoryModel : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        [ObservableProperty]
        private bool isSelected;
    }
}

