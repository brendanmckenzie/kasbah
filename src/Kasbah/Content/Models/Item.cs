using System;

namespace Kasbah.Content.Models
{
    public class Item : IItem
    {
        public Guid Id { get; set; }

        public Node Node { get; set; }

        public long Version { get; set; }
    }
}
