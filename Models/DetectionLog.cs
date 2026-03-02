using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis.Mobile.Models
{
    public class BreadDetection
    {
        public string BreadType { get; set; } = string.Empty;
        public double Probability { get; set; }
        public bool IsMoldDetected { get; set; }
    }

    public class DetectionLog
    {
        public string BreadResultsJson { get; set; } = "{}";

        public List<BreadDetection> GetBreadResults()
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<BreadDetection>>(BreadResultsJson) ?? new List<BreadDetection>();
        }
    }
}
