using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;


[QueryProperty(nameof(Topic), "topic")]
public partial class LocalizePage : ContentPage
{
    public LocalizePage(LocalizeVewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
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