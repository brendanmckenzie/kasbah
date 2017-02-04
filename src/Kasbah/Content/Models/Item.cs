using System;

namespace Kasbah.Content.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public Node Node { get; set; }
        public int Version { get; set; }
    }
}
