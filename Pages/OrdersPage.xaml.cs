using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace FMMSRestaurant.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private bool _isInitialized;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            // TODO: Load initial data here (e.g., list of orders)
            await Task.CompletedTask;
        }
    }
}
