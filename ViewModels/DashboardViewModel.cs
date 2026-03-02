using System.ComponentModel;
using System.Net.Http.Json;
using Thesis.Mobile.Models;
using Thesis.Mobile.Services; 
using Thesis.Mobile.Pages;

namespace Thesis.Mobile.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;

        public DashboardViewModel()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://thesis-production-8006.up.railway.app/")
            };

            // Load initial data
            LoadData();

            // Connect SignalR
            SetupSignalR();
        }

        #region Properties

        // Environmental Status
        // Change your setter logic or the way you receive data to look like this:
        private string temperatureText = "-- °C";
        public string TemperatureText
        {
            get => temperatureText;
            set
            {
                // Ensure 'value' is just "28 °C", NOT "Temperature: 28 °C"
                temperatureText = value;
                OnPropertyChanged(nameof(TemperatureText));
            }
        }

        private string humidityText = "-- %";
        public string HumidityText
        {
            get => humidityText;
            set
            {
                // Ensure 'value' is just "70 %", NOT "Humidity: 70 %"
                humidityText = value;
                OnPropertyChanged(nameof(HumidityText));
            }
        }

        // System Banner
        private string systemBannerText = "System OK";
        public string SystemBannerText
        {
            get => systemBannerText;
            set { systemBannerText = value; OnPropertyChanged(nameof(SystemBannerText)); }
        }

        private Color systemBannerColor = Colors.Green;
        public Color SystemBannerColor
        {
            get => systemBannerColor;
            set { systemBannerColor = value; OnPropertyChanged(nameof(SystemBannerColor)); }
        }

        // Bread Tiles
        private string spanishBreadStatus = "Safe";
        public string SpanishBreadStatus
        {
            get => spanishBreadStatus;
            set { spanishBreadStatus = value; OnPropertyChanged(nameof(SpanishBreadStatus)); }
        }

        private Color spanishBreadColor = Colors.Green;
        public Color SpanishBreadColor
        {
            get => spanishBreadColor;
            set { spanishBreadColor = value; OnPropertyChanged(nameof(SpanishBreadColor)); }
        }

        private string ensaymadaStatus = "Safe";
        public string EnsaymadaStatus
        {
            get => ensaymadaStatus;
            set { ensaymadaStatus = value; OnPropertyChanged(nameof(EnsaymadaStatus)); }
        }

        private Color ensaymadaColor = Colors.Green;
        public Color EnsaymadaColor
        {
            get => ensaymadaColor;
            set { ensaymadaColor = value; OnPropertyChanged(nameof(EnsaymadaColor)); }
        }

        private string panDeCocoStatus = "Safe";
        public string PanDeCocoStatus
        {
            get => panDeCocoStatus;
            set { panDeCocoStatus = value; OnPropertyChanged(nameof(PanDeCocoStatus)); }
        }

        private Color panDeCocoColor = Colors.Green;
        public Color PanDeCocoColor
        {
            get => panDeCocoColor;
            set { panDeCocoColor = value; OnPropertyChanged(nameof(PanDeCocoColor)); }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region API Load

        public async void LoadData()
        {
            try
            {
                // Get latest sensor reading
                var sensor = await _httpClient.GetFromJsonAsync<SensorReading>("api/sensor/latest");
                if (sensor != null)
                {
                    TemperatureText = $"{sensor.Temperature} °C";
                    HumidityText = $"{sensor.Humidity} %";

                    if (sensor.Temperature > 30 || sensor.Humidity > 75)
                    {
                        SystemBannerText = "Warning: Environment exceeds thresholds!";
                        SystemBannerColor = Colors.Red;
                    }
                    else
                    {
                        SystemBannerText = "System OK";
                        SystemBannerColor = Colors.Green;
                    }
                }

                // Get latest detection log
                var detection = await _httpClient.GetFromJsonAsync<DetectionLog>("api/detection/latest");
                if (detection != null)
                    UpdateBreadTiles(detection);
            }
            catch
            {
                SystemBannerText = "Error fetching data";
                SystemBannerColor = Colors.Gray;
            }
        }

        #endregion

        #region SignalR

        private void SetupSignalR()
        {
            var hubUrl = "https://thesis-production-8006.up.railway.app/alerthub";
            var service = SignalRService.Instance;

            service.OnMoldDetected += (log) =>
            {
                UpdateBreadTiles(log);

                // Show popup alert
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var breadWithAlert = log.GetBreadResults().FirstOrDefault(b => b.IsMoldDetected);
                    if (breadWithAlert != null)
                    {
                        bool view = await Application.Current.MainPage.DisplayAlert(
                            "⚠ Warning!",
                            $"{breadWithAlert.BreadType} mold detected.\nProbability: {breadWithAlert.Probability:F1}%",
                            "View Detection",
                            "Ignore"
                        );

                        if (view)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new DetectionPage());
                        }
                    }
                    

                });
            };

            _ = service.ConnectAsync(hubUrl); // fire-and-forget
        }

        #endregion

        #region Helper

        private void UpdateBreadTiles(DetectionLog detection)
        {
            var breads = detection.GetBreadResults();

            foreach (var bread in breads)
            {
                switch (bread.BreadType)
                {
                    case "Spanish Bread":
                        SpanishBreadStatus = bread.IsMoldDetected ? "Warning" : "Safe";
                        SpanishBreadColor = bread.IsMoldDetected ? Colors.Red : Colors.Green;
                        OnPropertyChanged(nameof(SpanishBreadStatus));
                        OnPropertyChanged(nameof(SpanishBreadColor));
                        break;

                    case "Ensaymada":
                        EnsaymadaStatus = bread.IsMoldDetected ? "Warning" : "Safe";
                        EnsaymadaColor = bread.IsMoldDetected ? Colors.Red : Colors.Green;
                        OnPropertyChanged(nameof(EnsaymadaStatus));
                        OnPropertyChanged(nameof(EnsaymadaColor));
                        break;

                    case "Pan de Coco":
                        PanDeCocoStatus = bread.IsMoldDetected ? "Warning" : "Safe";
                        PanDeCocoColor = bread.IsMoldDetected ? Colors.Red : Colors.Green;
                        OnPropertyChanged(nameof(PanDeCocoStatus));
                        OnPropertyChanged(nameof(PanDeCocoColor));
                        break;
                }
            }
        }

        #endregion
    }
}