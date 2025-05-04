using CommunityToolkit.Mvvm.ComponentModel;

namespace FMMSRestaurant.Models
{
    public partial class MenuCategoryModel : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsSelected { get; set; } // Added for XAML binding
    }
}
