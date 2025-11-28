using CommunityToolkit.Mvvm.ComponentModel;

namespace Lacalizer.Mobile.ViewModels;

public partial class CameraVewModel : ObservableObject
{

    [ObservableProperty]
    private string _selectedTopic;
    public CameraVewModel()
    {
        SelectedTopic = string.Empty;
    }
}
