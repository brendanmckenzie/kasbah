using System.Collections.Generic;

namespace Kasbah.Web.Analytics.Models
{
    public class TrackEventRequest
    {
        public string Type { get; set; }

        public string Source { get; set; }

        public IDictionary<string, string> Data { get; set; }
    }
}
