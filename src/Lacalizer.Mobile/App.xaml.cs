using Localizer.Mobile.Services.Device.Platform;
using Localizer.Mobile.Services.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Lacalizer.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
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

        // let's set the initial theme already during the app start
        SetTheme();

        // subscribe to changes in the settings
        SettingsService.Instance.PropertyChanged += OnSettingsPropertyChanged!;
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
                DeviceService.Instance.SetStatusBarColor(Colors.Black, false);
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