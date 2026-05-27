using Android.App;
using Android.Content.PM;
using Lacalizer.Mobile.Navigation;

namespace Lacalizer.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override void OnBackPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var handled = await NativeBackButtonService.Instance.HandleBackRequestedAsync();
            if (!handled)
                base.OnBackPressed();   
        });
    }
}

 
