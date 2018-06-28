using System;
using System.Collections.Generic;

namespace Kasbah.Analytics.Models
{
    public class Session
    {
        public Guid Id { get; set; }

        public IDictionary<string, object> Attributes { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActivity { get; set; }
    }
}
