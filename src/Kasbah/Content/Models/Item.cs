using System;

namespace Kasbah.Content.Models
{
    public interface IItem
    {
        Guid Id { get; set; }
        Node Node { get; set; }
        long Version { get; set; }
    }

    public class Item : IItem
    {
        public Guid Id { get; set; }
        public Node Node { get; set; }
        public long Version { get; set; }
    }
}
