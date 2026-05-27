using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using System.Windows.Input;

namespace Lacalizer.Mobile.Helpers;

public static class PageBackRoutingExtensions
{
    public static void RegisterBackHandler(this Page page)
    {
        page.Appearing += OnPageAppearing;
        page.Disappearing += OnPageDisappearing;
    }

    private static void OnPageAppearing(object? sender, EventArgs e)
    {
        if (sender is not Page page)
            return;

        var vm = page.BindingContext;

        var backCmdProp = vm?
            .GetType()
            .GetProperties()
            .FirstOrDefault(p => p.Name == "BackCommand" || p.Name == "BackCommandAsync");

        if (backCmdProp != null)
        {
            var command = backCmdProp.GetValue(vm);

            NativeBackButtonService.Instance.RegisterBackHandler(async () =>
            {
                if (command is IAsyncRelayCommand asyncCmd)
                {
                    await asyncCmd.ExecuteAsync(null);
                    return true;
                }
                if (command is ICommand cmd)
                {
                    cmd.Execute(null);
                    return true;
                }

                return false;
            });
        }
    }

    private static void OnPageDisappearing(object? sender, EventArgs e)
    {
        NativeBackButtonService.Instance.UnregisterBackHandler();
    }
}

