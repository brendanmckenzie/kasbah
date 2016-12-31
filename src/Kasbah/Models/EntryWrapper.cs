using System;

namespace Kasbah.Models
{
    public class EntryWrapper<T>
    {
        public Guid Id { get; set; }
        public T Entry { get; set; }
    }
}