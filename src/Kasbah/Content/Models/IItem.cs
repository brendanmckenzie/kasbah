using System;

namespace Kasbah.Content.Models
{
    public interface IItem
    {
        Guid Id { get; set; }

        Node Node { get; set; }

        long Version { get; set; }
    }
}
