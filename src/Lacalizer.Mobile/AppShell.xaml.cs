
using Lacalizer.Mobile.Navigation;

namespace Lacalizer.Mobile;
public partial class AppShell : Shell
{
    private readonly INavigationService _navigationService;
    public AppShell()
    {
        InitializeComponent();
        foreach (var route in Routes.RouteTypeMap)
        {
            Routing.RegisterRoute(route.Key, route.Value);
        }
    }
    private async void Shell_Loaded(object sender, EventArgs e)
    {
        if (Current?.CurrentState?.Location.OriginalString == "/")
        {
            await GoToAsync(Routes.MainPage); 
        }
    }
}