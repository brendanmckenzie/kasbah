using System;
using System.Collections.Generic;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public class ComponentDefinition
    {
        public string Alias { get; set; }

        public string Hint { get; set; }

        public Type Control { get; set; }

        public TypeDefinition Properties { get; set; }

        public IEnumerable<PlaceholderDefinition> Placeholders { get; set; }
    }
}
