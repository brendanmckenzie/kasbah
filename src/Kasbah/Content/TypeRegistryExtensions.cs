using System;
using Kasbah.Content.Models;

namespace Kasbah.Content
{
    public static class TypeRegistryExtensions
    {
        public static TypeRegistry Register<T>(this TypeRegistry registry, Action<TypeDefinitionBuilder> configure = null)
            where T : Item
        {
            var builder = new TypeDefinitionBuilder<T>(registry.PropertyMapper);

            configure?.Invoke(builder);

            registry.RegisterType(builder.Build());

            return registry;
        }
    }
}
