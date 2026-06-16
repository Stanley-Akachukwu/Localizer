using Lacalizer.Mobile.Helpers;
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Lacalizer.Mobile.Views;

public partial class ReelPage : ContentPage
{
    private readonly ReelViewModel _vm;
    public ReelPage(ReelViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.RegisterBackHandler();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (_vm?.LoadVideosCommand == null)
                return;

            _vm.Videos ??= new ObservableCollection<ReelVideoModel>();

            if (_vm.Videos.Count == 0)
            {
                await _vm.LoadVideosCommand.ExecuteAsync(null);

                if (_vm.Videos.Count == 0)
                {
                    bool createContext = await Shell.Current.DisplayAlert(
                        "No Available Videos",
                        "No available video to participate in.\n\nMake a new context?",
                        "Create Context",
                        "Cancel");

                    if (createContext)
                    {
                        await Shell.Current.GoToAsync(nameof(ContextsPage));
                    }

                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading videos: {ex}");
        }
    }


    private void ItemsView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        var itemIndex = e.CenterItemIndex;
        _vm.Videos[itemIndex].IsPlaying = true;
        _vm.ContextText = _vm.Videos[itemIndex].ContextText;
        _vm.VideoContextId = _vm.Videos[itemIndex].VideoContextId;
        _vm.VideoItemId = _vm.Videos[itemIndex].VideoItemId;

        foreach (var myModel in _vm.Videos)
        {
            if (myModel != _vm.Videos[itemIndex])
            {
                myModel.IsPlaying = false;
            }
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is ReelViewModel vm)
        {
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_vm.SelectedVideo == null)
            return;

        if (e.PropertyName == nameof(_vm.SelectedVideo.IsCommentsVisible))
        {
            if (CommentPanel == null)
                return;

            if (_vm.SelectedVideo.IsCommentsVisible)
            {
                CommentPanel.IsVisible = true;
                await CommentPanel.TranslateTo(0, 0, 250, Easing.SinOut);
            }
            else
            {
                await CommentPanel.TranslateTo(0, 500, 250, Easing.SinIn);
                CommentPanel.IsVisible = false;
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