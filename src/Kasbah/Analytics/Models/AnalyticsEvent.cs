using System;
using System.Collections.Generic;

namespace Kasbah.Analytics.Models
{
    public class AnalyticsEvent
    {
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Source { get; set; }
        public IDictionary<string, object> Data { get; set; }
        public Guid Persona { get; set; }
        public Guid Session { get; set; }
    }
}