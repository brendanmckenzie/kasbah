using System.Collections.Generic;

namespace Kasbah.Web.Models
{
    public class Component
    {
        public string Control { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}
