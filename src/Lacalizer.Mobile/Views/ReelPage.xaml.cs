using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class ReelPage : ContentPage
{
    private readonly ReelViewModel _vm;
    private readonly INavigationService _navigationService;
    public ReelPage(ReelViewModel vm, INavigationService navigationService)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _navigationService = navigationService;

    }

    private void ItemsView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        
        var itemIndex = e.CenterItemIndex;

        _vm.Videos[itemIndex].IsPlaying = true;
        _vm.SelectedTopic = _vm.Videos[itemIndex].Topic;
        foreach (var myModel in _vm.Videos)
        {
            if (myModel != _vm.Videos[itemIndex])
            {
                myModel.IsPlaying = false;
            }
        }
    }

    private async void OnRecordVideoClicked(object sender, EventArgs e)
    {
        // string topic = lblTopic.Text;
        await _navigationService.GoToAsync($"{Routes.CameraPage}?topic={_vm.SelectedTopic}");
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        foreach (var video in _vm.Videos)
        {
            video.IsPlaying = false;
        }
    }
}