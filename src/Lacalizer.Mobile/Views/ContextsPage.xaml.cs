using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile.Views;

public partial class ContextsPage : ContentPage
{
    private readonly ContextsViewModel _vm;

    public ContextsPage(ContextsViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;

        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_vm.Contexts.Count == 0)
            await _vm.LoadContextsCommand.ExecuteAsync(null);
    }
}