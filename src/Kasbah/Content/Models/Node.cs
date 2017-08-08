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

        public long? PublishedVersion { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public Lazy<IEnumerable<Node>> Children { get; set; }
    }
}
