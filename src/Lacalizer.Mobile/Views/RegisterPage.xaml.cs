using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Email = "stan@yahoo.com";
        viewModel.FirstName = "stan";
        viewModel.PhoneNumber = "08033208157";
        viewModel.LastName = "Aka";
        viewModel.Password = "Edu@123";    
        BindingContext = viewModel;
    }
}