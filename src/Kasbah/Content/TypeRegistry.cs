using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kasbah.Content.Models;

namespace Kasbah.Content
{
    public class TypeRegistry
    {
        readonly ICollection<TypeDefinition> _types;

        public TypeRegistry()
        {
            _types = new List<TypeDefinition>();
        }

        public void RegisterType(TypeDefinition type)
        {
            if (!typeof(Item).GetTypeInfo().IsAssignableFrom(Type.GetType(type.Alias)))
            {
                throw new InvalidOperationException($"All content models must inherit from {typeof(Item).FullName}");
            }

            _types.Add(type);
        }

        public IEnumerable<TypeDefinition> ListTypes()
            => _types.AsEnumerable();

        public TypeDefinition GetType(string type)
            => _types.SingleOrDefault(ent => ent.Alias == type);

        public IEnumerable<TypeDefinition> GetTypesThatImplement<TType>()
            where TType : IItem
            => _types.Where(ent => typeof(TType).IsAssignableFrom(Type.GetType(ent.Alias)));
    }

    public static class TypeRegistryExtensions
    {
        public static TypeRegistry Register<T>(this TypeRegistry registry, Action<TypeDefinitionBuilder> configure = null)
            where T : Item
        {
            var builder = new TypeDefinitionBuilder<T>();

            configure?.Invoke(builder);

            registry.RegisterType(builder.Build());

            return registry;
        }
    }
}
