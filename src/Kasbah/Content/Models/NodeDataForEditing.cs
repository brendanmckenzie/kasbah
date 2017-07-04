using System.Collections.Generic;

namespace Kasbah.Content.Models
{
    public class NodeDataForEditing
    {
        public Node Node { get; set; }

        public IDictionary<string, object> Data { get; set; }

        public TypeDefinition Type { get; set; }
    }
}
