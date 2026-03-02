using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Thesis.Mobile.ViewModels
{
    public class DetectionViewModel : INotifyPropertyChanged
    {
        private string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set { _imageUrl = value; OnPropertyChanged(); }
        }

        public ObservableCollection<BreadResult> BreadResults { get; set; } = new();

        private readonly HttpClient _httpClient;

        public DetectionViewModel()
        {
            _httpClient = new HttpClient();
        }

        public async Task LoadDetectionDataAsync()
        {
            try
            {
                string url = "https://thesis-production-8006.up.railway.app/api/detection/latest";

                var data = await _httpClient.GetFromJsonAsync<DetectionResponse>(url);

                if (data != null)
                {
                    ImageUrl = data.ImageUrl;

                    // Clear existing results
                    BreadResults.Clear();

                    // Add each bread result
                    foreach (var bread in data.BreadResults)
                    {
                        BreadResults.Add(bread);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching detection data: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Helper classes for deserialization
    public class DetectionResponse
    {
        public string ImageUrl { get; set; }
        public BreadResult[] BreadResults { get; set; }
    }

    public class BreadResult
    {
        public string BreadType { get; set; }
        public double Probability { get; set; }
        public bool IsMoldDetected { get; set; }

        public string Status => IsMoldDetected ? "Mold Detected" : "Safe";
        public string Color => IsMoldDetected ? "Red" : "Green";
    }
}