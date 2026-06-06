using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.ViewModels;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.Views;

[QueryProperty(nameof(VideoContextId), "videoContextId")]
public partial class ParticipationPage : ContentPage
{
    private readonly ParticipationViewModel _vm;
    public ParticipationPage(ParticipationViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

    }

    public string VideoContextId
    {
        get => (BindingContext as ParticipationViewModel)?.VideoContextId;
        set
        {
            var vm = BindingContext as ParticipationViewModel;
            if (vm != null)
                vm.VideoContextId = value;
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

        if (_vm.Videos == null) _vm.Videos = new ObservableCollection<ParticipationVideoModel>();

        if (_vm.Videos.Count == 0)
            await _vm.LoadVideosCommand.ExecuteAsync(null);
    }


    private void ItemsView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {

        var itemIndex = e.CenterItemIndex;

        _vm.Videos[itemIndex].IsPlaying = true;
        _vm.SelectedContext = _vm.Videos[itemIndex].ContextText;
        _vm.VideoContextId = _vm.Videos[itemIndex].VideoContextId;
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

        if (_vm?.Videos == null)
            return;

        foreach (var video in _vm.Videos)
            video.IsPlaying = false;
    }
}