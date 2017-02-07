using System;
using System.Collections.Generic;

namespace Kasbah.Analytics.Models
{
    public class AnalyticsEvent
    {
        public Guid Profile { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Source { get; set; }
        public IDictionary<string, string> Data { get; set; }
    }
}
