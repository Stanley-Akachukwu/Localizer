using Lacalizer.Mobile.Helpers;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;


[QueryProperty(nameof(Topic), "topic")]
[QueryProperty(nameof(VideoTopicId), "videoTopicId")]

public partial class LocalizePage : ContentPage
{
    private readonly INavigationService _navigationService;
    public LocalizePage(LocalizeVewModel vm, INavigationService navigationService)
    {
        InitializeComponent();
        BindingContext = vm;
        _navigationService = navigationService;
        this.RegisterBackHandler();
    }
     
    public string Topic
    {
        get => (BindingContext as LocalizeVewModel)?.SelectedTopic;
        set
        {
            var vm = BindingContext as LocalizeVewModel;
            if (vm != null)
                vm.SelectedTopic = value;
        }
    }
    public string VideoTopicId
    {
        get => (BindingContext as LocalizeVewModel)?.VideoTopicId;
        set
        {
            var vm = BindingContext as LocalizeVewModel;
            if (vm != null)
                vm.VideoTopicId = value;
        }
    }
    private void CameraViewLoaded(object sender, EventArgs e)
    {
        (BindingContext as LocalizeVewModel)?.RegisterCameraView(cameraView);
    }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        cameraView.Camera = cameraView.Cameras.LastOrDefault();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (cameraView.Cameras.Count > 0)
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            }
        });
    }
}