using System.Collections.Generic;

namespace Kasbah.Web.Analytics.Models
{
    public class TrackSessionActivityRequest
    {
        public string Type { get; set; }

        public IDictionary<string, object> Data { get; set; }
    }
}
