using Thesis.Mobile.Pages;
namespace Thesis.Mobile

{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Pages.DetectionPage), typeof(Pages.DetectionPage));
        }
    }
}
