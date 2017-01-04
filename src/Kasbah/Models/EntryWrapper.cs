using System;

namespace Kasbah.Models
{
    public class EntryWrapper<T>
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public T Source { get; set; }
    }
}