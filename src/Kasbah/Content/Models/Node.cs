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
        public IDictionary<string, object> Data { get; set; }
    }

    public class NodeTaxonomy
    {
        public IEnumerable<Guid> Ids { get; set; }
        public IEnumerable<string> Aliases { get; set; }
    }
}