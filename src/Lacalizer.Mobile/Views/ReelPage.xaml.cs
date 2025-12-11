using Lacalizer.Mobile.Helpers;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;
using System.Collections.ObjectModel;

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
        this.RegisterBackHandler();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();


        if (_vm == null)
        {
            Console.WriteLine("ReelViewModel is null!");
            return;
        }

        if (_vm.LoadVideosCommand == null)
        {
            Console.WriteLine("LoadVideosCommand is null!");
            return;
        }

        if (_vm.Videos == null) _vm.Videos = new ObservableCollection<ReelVideoModel>();

        if (_vm.Videos.Count == 0)
            await _vm.LoadVideosCommand.ExecuteAsync(null);
    }


    private void ItemsView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        var itemIndex = e.CenterItemIndex;
        _vm.Videos[itemIndex].IsPlaying = true;
        _vm.SelectedTopic = _vm.Videos[itemIndex].Topic;
        _vm.VideoTopicId = _vm.Videos[itemIndex].VideoTopicId;


        foreach (var myModel in _vm.Videos)
        {
            if (myModel != _vm.Videos[itemIndex])
            {
                myModel.IsPlaying = false;
            }
        }
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