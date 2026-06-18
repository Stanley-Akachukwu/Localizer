using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Services.Users;
using Localizer.Mobile.Services.Device;

namespace Lacalizer.Mobile.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDeviceService _deviceService;
    private readonly AuthStateProvider _authStateProvider;
    public MainViewModel(IDeviceService deviceService, 
        AuthStateProvider authStateProvider)
    {
        _deviceService = deviceService;
        _authStateProvider = authStateProvider;
       logout();
    }

    [RelayCommand]
    public void SetHighBrightness()
    {
        _deviceService.SetScreenBrightness(1.0f);
    }

    public void logout()
    {
        _authStateProvider.LogoutAsync();
    }

    [RelayCommand]
    public void SetLowBrightness()
    {
        _deviceService.SetScreenBrightness(0.1f);
    }

    [ObservableProperty]
    private double _radius = 100.0;

    [ObservableProperty]
    private bool isVideoPlaying;
}
