using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class MainPage
{
    private readonly INavigationService _navigationService;

    public MainPage(MainViewModel viewModel, INavigationService navigationService)
    {
        _navigationService = navigationService;

        InitializeComponent();
        BindingContext = viewModel;
    }

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await _navigationService.GoToAsync(nameof(ReelPage));
    //}
}

