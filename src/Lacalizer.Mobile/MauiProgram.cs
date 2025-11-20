using CommunityToolkit.Maui;
using epj.Expander.Maui;
using epj.RouteGenerator;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;
using Lacalizer.Mobile.Views;
using Localizer.Mobile.Services;
using Localizer.Mobile.Services.Audio;
using Localizer.Mobile.Services.Device;
using Localizer.Mobile.Services.Device.Platform;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;

namespace Lacalizer.Mobile;

[AutoRoutes("Page")]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureMopups()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined-Regular");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons-Regular");
                fonts.AddFont("Strande2.ttf", "Strande2");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddTransient<ReelViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<IDeviceService>(DeviceService.Instance);
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddTransient<ReelPage>();

        var app = builder.Build();

        ServiceHelper.Initialize(app.Services);
        Expander.EnableAnimations();

        return app;
    }
}
