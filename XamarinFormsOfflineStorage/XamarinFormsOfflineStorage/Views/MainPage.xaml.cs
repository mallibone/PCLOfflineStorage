using Xamarin.Forms;
using XamarinFormsOfflineStorage.ViewModels;

namespace XamarinFormsOfflineStorage.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            _viewModel = App.Locator.Main;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.Init();
            base.OnAppearing();
        }
    }
}
