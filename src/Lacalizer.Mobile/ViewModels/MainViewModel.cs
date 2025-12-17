using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using Localizer.Mobile.Services.Audio;
using Localizer.Mobile.Services.Device;

namespace Lacalizer.Mobile.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDeviceService _deviceService;
    private readonly IAudioService _audioService;
    private readonly INavigationService _navigationService;

    public MainViewModel(IDeviceService deviceService, INavigationService navigationService)
    {
        _deviceService = deviceService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public void SetHighBrightness()
    {
        _deviceService.SetScreenBrightness(1.0f);
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
