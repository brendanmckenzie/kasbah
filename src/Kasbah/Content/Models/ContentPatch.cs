using System;
using System.Collections.Generic;

namespace Kasbah.Content.Models
{
    public class ContentPatch
    {
        public Guid Id { get; set; }
        // public TItem Patch { get; set; }
        public IDictionary<string, object> Values { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
        public IDictionary<string, long> Bias { get; set; }
        public long Weight { get; set; }
    }
}
