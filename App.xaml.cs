using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Thesis.Mobile.Pages;

namespace Thesis.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new DashboardPage());
        }
    }
}