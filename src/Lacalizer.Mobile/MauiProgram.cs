using Camera.MAUI;
using CommunityToolkit.Maui;
using epj.Expander.Maui;
using epj.RouteGenerator;
using Lacalizer.Mobile.Extensions;
using Localizer.Mobile.Services;
using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Plugin.Maui.ScreenRecording;
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
            .UseScreenRecording()
             .UseMauiCameraView()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitCamera()
            .UseMauiCommunityToolkitMediaElement(
                isAndroidForegroundServiceEnabled: true
            ).ConfigureMopups()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined-Regular");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons-Regular");
                fonts.AddFont("Strande2.ttf", "Strande2");
            });

        // Configure Client-Side Logging
        builder.Logging
            .AddTraceLogger(_ => { })
            .AddInMemoryLogger(_ => { }) // Required for the built-in UI
            .AddStreamingFileLogger(options =>
            {
                options.RetainDays = 7; // Keep a week of logs
                options.MinLevel = LogLevel.Information;
            });
        // Configure Client-Side Logging
        var getAssembly = Assembly.GetExecutingAssembly();
        using var appjson = getAssembly.GetManifestResourceStream("Lacalizer.Mobile.appsettings.json");
        var newConfig = new ConfigurationBuilder()
            .AddJsonStream(appjson)
            .Build();
        builder.Configuration.AddConfiguration(newConfig);
         

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.ConfigureServices();
        var app = builder.Build();

        ServiceHelper.Initialize(app.Services);
        Expander.EnableAnimations();

        return app;
    }
}
