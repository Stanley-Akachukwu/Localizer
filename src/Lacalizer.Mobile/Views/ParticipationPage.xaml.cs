using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.ViewModels;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.Views;

[QueryProperty(nameof(VideoTopicId), "videoTopicId")]
public partial class ParticipationPage : ContentPage
{
    private readonly ParticipationViewModel _vm;
    public ParticipationPage(ParticipationViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

    }

    public string VideoTopicId
    {
        get => (BindingContext as ParticipationViewModel)?.VideoTopicId;
        set
        {
            var vm = BindingContext as ParticipationViewModel;
            if (vm != null)
                vm.VideoTopicId = value;
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_vm == null)
        {
            Console.WriteLine("ParticipationViewModel is null!");
            return;
        }

        if (_vm.LoadVideosCommand == null)
        {
            Console.WriteLine("LoadVideosCommand is null!");
            return;
        }

        if (_vm.Videos == null) _vm.Videos = new ObservableCollection<VideoModel>();

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