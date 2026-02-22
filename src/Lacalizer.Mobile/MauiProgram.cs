using Camera.MAUI;
using CommunityToolkit.Maui;
using epj.Expander.Maui;
using epj.RouteGenerator;
using Lacalizer.Mobile.Extensions;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Lacalizer.Mobile.ViewModels;
using Lacalizer.Mobile.Views;
using Localizer.Mobile.Services;
using Localizer.Mobile.Services.Audio;
using Localizer.Mobile.Services.Device;
using Localizer.Mobile.Services.Device.Platform;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using System;
using System.Reflection;

namespace Lacalizer.Mobile;

[AutoRoutes("Page")]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
             .UseMauiCameraView()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitCamera()
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


        var getAssembly = Assembly.GetExecutingAssembly();
        using var appjson = getAssembly.GetManifestResourceStream("Lacalizer.Mobile.appsettings.json");
        var newConfig = new ConfigurationBuilder()
            .AddJsonStream(appjson)
            .Build();
        builder.Configuration.AddConfiguration(newConfig);
         

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        builder.Services.AddSingleton<IDeviceService>(DeviceService.Instance);
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<ReelViewModel>();
        builder.Services.AddTransient<ReelPage>();

        builder.Services.AddTransient<LocalizeVewModel>();
        builder.Services.AddTransient<LocalizePage>();

        builder.Services.AddTransient<ParticipationViewModel>();
        builder.Services.AddTransient<ParticipationPage>();

        builder.Services.AddMemoryCache();

        var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

        builder.Services.AddHttpClient<IVideoService, VideoService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        builder.Services.AddHttpClient<ICommentService, CommentService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });
        builder.ConfigureServices();
        var app = builder.Build();

        ServiceHelper.Initialize(app.Services);
        Expander.EnableAnimations();

        return app;
    }
}
