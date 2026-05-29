using Lacalizer.Mobile.Services.Users;
using Lacalizer.Mobile.Views;
using Localizer.Mobile.Services.Device.Platform;
using Localizer.Mobile.Services.Settings;
using System.ComponentModel;

namespace Lacalizer.Mobile;

public partial class App : Application
{
    public App(SessionService sessionService)
    {
        InitializeComponent();
        //ClearCache();

        CheckAuthentication(sessionService);

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Console.WriteLine("AppDomain");
        };
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            Console.WriteLine("TaskScheduler");
        };
        Dispatcher.Dispatch(new Action(() => { Console.WriteLine("test"); }));
        Application.Current.Dispatcher.Dispatch(new Action(() => { Console.WriteLine("test"); }));

        SetTheme();

        SettingsService.Instance.PropertyChanged += OnSettingsPropertyChanged!;
    }

    private void ClearCache()
    {
        try
        {
            var cacheDir = FileSystem.CacheDirectory;

            if (Directory.Exists(cacheDir))
            {
                Directory.Delete(cacheDir, true);
            }

            Directory.CreateDirectory(cacheDir);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Cache clear failed: {ex}");
        }
    }
    private async void CheckAuthentication(
        SessionService sessionService)
    {
        var isLoggedIn = await sessionService.IsLoggedInAsync();

        if (!isLoggedIn)
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());

    private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingsService.Theme))
        {
            SetTheme();
        }
    }

    private void SetTheme()
    {
        UserAppTheme = SettingsService.Instance?.Theme != null
                     ? SettingsService.Instance.Theme.AppTheme
                     : AppTheme.Unspecified;

        switch (UserAppTheme)
        {
            case AppTheme.Light:
                DeviceService.Instance.SetStatusBarColor(Colors.White, true);
                break;
            case AppTheme.Dark:
                DeviceService.Instance.SetStatusBarColor(Colors.White, false);
                break;
            case AppTheme.Unspecified when RequestedTheme == AppTheme.Light:
                DeviceService.Instance.SetStatusBarColor(Colors.White, true);
                break;
            case AppTheme.Unspecified:
                DeviceService.Instance.SetStatusBarColor(Colors.Black, false);
                break;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnResume()
    {
        base.OnResume();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
    }
}