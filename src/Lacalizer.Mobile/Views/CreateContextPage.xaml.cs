using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class CreateContextPage : ContentPage
{
    public CreateContextPage(CreateContextViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}