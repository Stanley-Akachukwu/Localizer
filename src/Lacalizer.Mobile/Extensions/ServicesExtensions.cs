
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Lacalizer.Mobile.ViewModels;
using Lacalizer.Mobile.Views;
using Localizer.Mobile.Services.Audio;
using Localizer.Mobile.Services.Device;
using Localizer.Mobile.Services.Device.Platform;

namespace Lacalizer.Mobile.Extensions;

public static class ServicesExtensions
{
    public static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
    {

        //#if WINDOWS
        //        builder.Services.TryAddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.Windows.NativeAudioService>();
        //#elif ANDROID
        //        builder.Services.TryAddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.Android.NativeAudioService>();
        //#elif MACCATALYST
        //        builder.Services.TryAddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.MacCatalyst.NativeAudioService>();
        //        builder.Services.TryAddSingleton< Platforms.MacCatalyst.ConnectivityService>();
        //#elif IOS
        //        builder.Services.TryAddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.iOS.NativeAudioService>();
        //#endif

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

        builder.Services.AddHttpClient<IVideoService, VideoService>(client =>
        {
            client.BaseAddress = new Uri("https://f38nk8m5-7078.uks1.devtunnels.ms/");
        });

        builder.Services.AddHttpClient<ICommentService, CommentService>(client =>
        {
            client.BaseAddress = new Uri("https://f38nk8m5-7078.uks1.devtunnels.ms/");
        });


        return builder;
    }
}
