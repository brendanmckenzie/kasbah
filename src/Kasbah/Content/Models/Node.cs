using System;
using System.Collections.Generic;

namespace Kasbah.Content.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public Guid? Parent { get; set; }
        public NodeTaxonomy Taxonomy { get; set; }
        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        // TODO: eventually perhaps move this to another index
        // and configure ES type mapping
        public IDictionary<string, object> Data { get; set; }
    }

    public class NodeTaxonomy
    {
        public IEnumerable<Guid> Ids { get; set; }
        public IEnumerable<string> Aliases { get; set; }
    }
}