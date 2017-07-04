using System;

namespace Kasbah.Content.Models
{
    public class QueryResultItem<TItem>
        where TItem : IItem
    {
        public Node Node { get; set; }

        public NodeData NodeData { get; set; }

        public Lazy<TItem> Item { get; set; }
    }
}
