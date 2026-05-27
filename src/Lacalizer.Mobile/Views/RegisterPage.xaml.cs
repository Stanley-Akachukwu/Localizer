using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}