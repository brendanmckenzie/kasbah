using System.Collections.Generic;

namespace Kasbah.Content.Models
{
    public class TypeDefinition
    {
        public string DisplayName { get; set; }
        public string Alias { get; set; }
        public IEnumerable<Field> Fields { get; set; }

        public class Field
        {
            const string DefaultCategory = "General";

            public string DisplayName { get; set; }
            public string Alias { get; set; }
            public string Type { get; set; }
            public string HelpText { get; set; }
            public string Category { get; set; } = DefaultCategory;
        }
    }
}