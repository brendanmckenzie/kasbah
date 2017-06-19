using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void RegisterComponent<TComponent>(this ComponentRegistry componentRegistry, Type properties = null)
            where TComponent : ViewComponent
        {
            var type = typeof(TComponent);

            var definition = new ComponentDefinition
            {
                Alias = type.Name,
                Control = type,
                Properties = properties
            };

            componentRegistry.RegisterComponent(definition);
        }
    }
}
