using System;

namespace Kasbah.Exceptions
{
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException(string index, Type type, Guid id, long? version)
            : base($"Entry not found in index '{index}' of type '{type.Name}' with id '{id}' (version: {version})")
        {
            Index = index;
            Type = type;
            Id = id;
            Version = version;
        }

        public string Index { get; set; }
        public Type Type { get; set; }
        public Guid Id { get; set; }
        public long? Version { get; set; }
    }
}
