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

            if (GetType(type.Alias) == null)
            {
                _types.Add(type);
            }
        }

        public IEnumerable<TypeDefinition> ListTypes()
            => _types.AsEnumerable();

        public TypeDefinition GetType(string type)
            => _types.SingleOrDefault(ent => String.Equals(ent.Alias.Split(',').First(), type.Split(',').First()));

        public IEnumerable<TypeDefinition> GetTypesThatImplement(Type type)
            => _types.Where(ent => type.IsAssignableFrom(Type.GetType(ent.Alias)));

        public IEnumerable<TypeDefinition> GetTypesThatImplement<TType>()
            where TType : IItem
            => GetTypesThatImplement(typeof(TType));
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
