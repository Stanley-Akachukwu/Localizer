using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        viewModel.PhoneNumber = "08033208157";
        viewModel.Password = "Edu@123";
        BindingContext = viewModel;
    }
}