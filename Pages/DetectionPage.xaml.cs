using Thesis.Mobile.ViewModels;
namespace Thesis.Mobile.Pages;


public partial class DetectionPage : ContentPage
{
    public DetectionViewModel ViewModel { get; set; }

    public DetectionPage()
    {
        InitializeComponent();
        ViewModel = new DetectionViewModel();
        BindingContext = ViewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fetch the latest detection data
        await ViewModel.LoadDetectionDataAsync();
    }
}