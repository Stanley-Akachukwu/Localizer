
using Lacalizer.Mobile.Helpers;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Users;
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
        var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
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

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();



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

        builder.Services.AddTransient<ReelVideoGroupViewModel>();
        builder.Services.AddTransient<ReelVideoGroupPage>();

        builder.Services.AddTransient<ContextsViewModel>();
        builder.Services.AddTransient<ContextsPage>();

        builder.Services.AddTransient<CreateContextViewModel>();
        builder.Services.AddTransient<CreateContextPage>();

        builder.Services.AddMemoryCache();

         

        builder.Services.AddTransient<CachedJwtHandler>();
        builder.Services.AddSingleton<ITokenProvider, TokenProvider>();
        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
        builder.Services.AddSingleton<AuthStateProvider>();

        builder.Services.AddScoped<IApiClient, ApiClient>();

        builder.Services.AddScoped<IVideoService, VideoService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IContextService, ContextService>();

        builder.Services.AddScoped<AuthService>();
        return builder;
    }
}
