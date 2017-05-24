using System;

namespace Kasbah.Content.Models
{
    public interface IItem
    {
        Guid Id { get; set; }
        Node Node { get; set; }
        int Version { get; set; }
    }

    public class Item : IItem
    {
        public Guid Id { get; set; }
        public Node Node { get; set; }
        public int Version { get; set; }
    }
}
