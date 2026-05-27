using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.ViewModels;
using Microsoft.Maui.Layouts;

namespace Lacalizer.Mobile.Views;

public partial class MainPage : ContentPage
{
    private readonly Random _random = new();

    public MainPage(MainViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        BindingContext = viewModel;

        StartFloatingWords();
    }

    private async void StartFloatingWords()
    {
        string[] words =
        {
            "Hola", "Bonjour", "Hallo", "Ciao","Igbo",
            "Ẹ káàbọ̀", "こんにちは", "مرحبا","Yoruba", "Привет", "नमस्ते", "Olá", "Hej",
            "Salam", "Zdravstvuyte", "Sawubona", "Merhaba","Swahili","Any",
            "Welcome", "Speak", "Learn", "Localize","Translate", "Global", "Connect", "Explore","Culture"
        };

        while (true)
        {
            CreateFloatingWord(words[_random.Next(words.Length)]);
            await Task.Delay(900);
        }
    }

    private async void CreateFloatingWord(string text)
    {
        var label = new Label
        {
            Text = text,
            FontSize = _random.Next(18, 36),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            Opacity = 0
        };

        AbsoluteLayout.SetLayoutBounds(
            label,
            new Rect(_random.NextDouble(), _random.NextDouble(), -1, -1));

        AbsoluteLayout.SetLayoutFlags(
            label,
            AbsoluteLayoutFlags.PositionProportional);

        OverlayLayer.Children.Add(label);

        await label.FadeTo(1, 500);
        await label.TranslateTo(
            _random.Next(-60, 60),
            _random.Next(-60, 60),
            2500,
            Easing.SinInOut);

        await label.FadeTo(0, 500);

        OverlayLayer.Children.Remove(label);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((MainViewModel)BindingContext).IsVideoPlaying = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ((MainViewModel)BindingContext).IsVideoPlaying = false;
    }

}

