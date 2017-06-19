using System;
using System.Collections.Generic;
using System.Linq;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web
{
    public class ComponentRegistry
    {
        readonly ICollection<ComponentDefinition> _components;

        public ComponentRegistry()
        {
            _components = new List<ComponentDefinition>();
        }

        public void RegisterComponent(ComponentDefinition component)
            => _components.Add(component);

        public IEnumerable<ComponentDefinition> ListComponents()
            => _components.AsEnumerable();

        public ComponentDefinition GetByAlias(string alias)
            => _components.FirstOrDefault(ent => String.Equals(ent.Alias, alias));
    }

    public static class ComponentRegistryExtensions
    {
        public static void RegisterComponent<TComponent, TProperties>(this ComponentRegistry componentRegistry, Action<TypeDefinitionBuilder> configure = null)
            where TComponent : ViewComponent
        {
            var type = typeof(TComponent);

            var builder = new TypeDefinitionBuilder<TProperties>();

            configure?.Invoke(builder);

            var definition = new ComponentDefinition
            {
                Alias = type.Name,
                Control = type,
                Properties = builder.Build()
            };

            componentRegistry.RegisterComponent(definition);
        }
        public static void RegisterComponent<TComponent>(this ComponentRegistry componentRegistry)
            where TComponent : ViewComponent
        {
            var type = typeof(TComponent);

            var definition = new ComponentDefinition
            {
                Alias = type.Name,
                Control = type
            };

            componentRegistry.RegisterComponent(definition);
        }
    }
}
