using Lacalizer.Mobile.ViewModels;

namespace Lacalizer.Mobile
{
    public partial class MainPage : ContentPage
    {
        private readonly ReelViewModel _vm;

        public MainPage(ReelViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        private void ItemsView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var itemIndex = e.CenterItemIndex;

            _vm.Videos[itemIndex].IsPlaying = true;

            foreach (var myModel in _vm.Videos)
            {
                if (myModel != _vm.Videos[itemIndex])
                {
                    myModel.IsPlaying = false;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            foreach (var video in _vm.Videos)
            {
                video.IsPlaying = false;
            }
        }
    }
}
