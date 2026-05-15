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
    }
}