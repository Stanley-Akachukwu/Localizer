using Lacalizer.Mobile.Helpers;
using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;


[QueryProperty(nameof(ContextText), "contextText")]
[QueryProperty(nameof(VideoContextId), "videoContextId")]
[QueryProperty(nameof(VideoItemId), "videoItemId")]

public partial class LocalizePage : ContentPage
{
    public LocalizePage(LocalizeVewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        this.RegisterBackHandler();
    }
     
    public string ContextText
    {
        get => (BindingContext as LocalizeVewModel)?.SelectedContext;
        set
        {
            var vm = BindingContext as LocalizeVewModel;
            if (vm != null)
                vm.SelectedContext = value;
        }
    }
    public string VideoContextId
    {
        get => (BindingContext as LocalizeVewModel)?.VideoContextId;
        set
        {
            var vm = BindingContext as LocalizeVewModel;
            if (vm != null)
                vm.VideoContextId = value;
        }
    }
    public string VideoItemId
    {
        get => (BindingContext as LocalizeVewModel)?.VideoItemId;
        set
        {
            var vm = BindingContext as LocalizeVewModel;
            if (vm != null)
                vm.VideoItemId = value;
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