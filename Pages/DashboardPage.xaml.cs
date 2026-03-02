using Microsoft.Maui.Controls;
using Thesis.Mobile.ViewModels;

namespace Thesis.Mobile.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private DashboardViewModel viewModel;

        public DashboardPage()
        {
            InitializeComponent(); 
            viewModel = new DashboardViewModel();
            BindingContext = viewModel;
        }

        private async void OnDetectionPageClicked(object sender, EventArgs e)
        {
            // Use Navigation.PushAsync because the app uses NavigationPage as MainPage
            await Navigation.PushAsync(new DetectionPage());
        }
    }
}