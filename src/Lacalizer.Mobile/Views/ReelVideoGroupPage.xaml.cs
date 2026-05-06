using Lacalizer.Mobile.Helpers;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;
public partial class ReelVideoGroupPage : ContentPage
{
    private readonly ReelVideoGroupViewModel _vm;
    private readonly INavigationService _navigationService;
    public ReelVideoGroupPage(ReelVideoGroupViewModel vm, INavigationService navigationService)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _navigationService = navigationService;
        this.RegisterBackHandler();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.LoadVideosCommand.ExecuteAsync(null);

        if (_vm.VideoGroup != null)
        {
            _ = _vm.PlaySequentially(_vm.VideoGroup);
        }
        //base.OnAppearing();

        //if (_vm?.LoadVideosCommand != null)
        //{
        //    await _vm.LoadVideosCommand.ExecuteAsync(null);
        //}

        //// Wait briefly to ensure binding/UI is ready
        //await Task.Delay(200);

        //var group = _vm?.VideoGroups?.FirstOrDefault();
        //if (group != null)
        //{
        //    _ = _vm.PlaySequentially(group);
        //}
    }
}