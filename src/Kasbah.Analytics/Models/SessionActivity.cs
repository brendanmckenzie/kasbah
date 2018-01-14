using System;
using Newtonsoft.Json.Linq;

namespace Kasbah.Analytics.Models
{
    public class SessionActivity
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; }

        public string Type { get; set; }

        public JObject Attributes { get; set; }

        public DateTime Created { get; set; }
    }
}
