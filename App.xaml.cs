using Microsoft.Extensions.DependencyInjection;

namespace FMMSRestaurant;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        MainPage = _serviceProvider.GetRequiredService<MainPage>();
    }
}