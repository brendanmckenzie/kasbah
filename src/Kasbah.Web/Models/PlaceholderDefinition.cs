using System.Collections.Generic;

namespace Kasbah.Web.Models
{
    public class PlaceholderDefinition
    {
        public string Alias { get; set; }

        public IEnumerable<string> AllowedControls { get; set; }
    }
}
