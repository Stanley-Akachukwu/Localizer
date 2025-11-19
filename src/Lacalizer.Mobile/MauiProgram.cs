using CommunityToolkit.Maui;
using epj.Expander.Maui;
using Lacalizer.Mobile.ViewModels;
using Localizer.Mobile.Services;
using Localizer.Mobile.Services.Audio;
using Localizer.Mobile.Services.Device;
using Localizer.Mobile.Services.Device.Platform;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;

namespace Lacalizer.Mobile
{
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
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<IDeviceService>(DeviceService.Instance);
            builder.Services.AddSingleton<IAudioService, AudioService>();

            //builder.Services.AddSingleton<INavigationService, NavigationService>();
            //builder.Services.AddSingleton<MainViewModel>();
            //builder.Services.AddTransient<BindingsViewModel>();
            //builder.Services.AddTransient<BindingsPage>();
            //builder.Services.AddTransient<ReelPage>();

            var app = builder.Build();

            ServiceHelper.Initialize(app.Services);
            Expander.EnableAnimations();

            return app;
        }
    }
}
