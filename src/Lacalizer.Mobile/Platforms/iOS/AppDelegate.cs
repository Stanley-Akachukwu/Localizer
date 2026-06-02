using Foundation;
using UIKit;

namespace Lacalizer.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    //public override bool FinishedLaunching(UIApplication app,NSDictionary options)
    //{
    //    AppDomain.CurrentDomain.UnhandledException +=
    //        (sender, e) =>
    //        {
    //            var ex = e.ExceptionObject as Exception;

    //            Console.WriteLine(ex);
    //        };

    //    TaskScheduler.UnobservedTaskException +=
    //        (sender, e) =>
    //        {
    //            Console.WriteLine(e.Exception);
    //            e.SetObserved();
    //        };

    //    return base.FinishedLaunching(app, options);
    //}
}
