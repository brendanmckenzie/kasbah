using System;
using System.Collections.Generic;

namespace Kasbah.Content.Models
{

    public class NodeTaxonomy
    {
        public IEnumerable<Guid> Ids { get; set; }
        public IEnumerable<string> Aliases { get; set; }
        public int Length { get; set; }
    }
}
