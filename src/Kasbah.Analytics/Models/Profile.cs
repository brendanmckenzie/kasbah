using System.Collections.Generic;

namespace Kasbah.Analytics.Models
{
    public class Profile
    {
        public IDictionary<string, object> Attributes { get; set; }

        public IEnumerable<Campaign> Campaigns { get; set; }

        public IEnumerable<Event> Events { get; set; }
    }
}
