using System.Collections.Generic;

namespace Kasbah.Web.Analytics.Models
{
    public class TrackSessionActivityRequest
    {
        public string Type { get; set; }

        public IDictionary<string, string> Data { get; set; }
    }
}
