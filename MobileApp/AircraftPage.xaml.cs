using Microsoft.Maui.Controls;

namespace MobileApp
{
    public partial class AircraftPage : ContentPage
    {
        public AircraftPage()
        {
            InitializeComponent();
        }

        private void OnViewAircraftClicked(object sender, EventArgs e)
        {
            DisplayAlert("Info", "Viewing aircraft data... (To be implemented)", "OK");
        }

        private void OnViewFlightsClicked(object sender, EventArgs e)
        {
            DisplayAlert("Info", "Viewing flight data... (To be implemented)", "OK");
        }

        private void OnCheckMaintenanceClicked(object sender, EventArgs e)
        {
            DisplayAlert("Info", "Checking maintenance records... (To be implemented)", "OK");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}