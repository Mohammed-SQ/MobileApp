namespace FMMSRestaurant
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.MinimumHeight = 760;
            window.MinimumWidth = 1280;
            return window;
        }
    }
}
