using System;
using System.Collections.Generic;

namespace Kasbah.Content.Models
{
    public class TypeDefinition
    {
        public string DisplayName { get; set; }
        public Type Type { get; set; }
        public IEnumerable<Field> Fields { get; set; }

        public class Field
        {
            public string DisplayName { get; set; }
            public string Alias { get; set; }
            public string Type { get; set; }
        }
    }
}