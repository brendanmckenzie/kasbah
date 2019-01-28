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

        public ComponentRegistry(PropertyMapper propertyMapper)
        {
            _components = new List<ComponentDefinition>();

            PropertyMapper = propertyMapper;
        }

        internal PropertyMapper PropertyMapper { get; private set; }

        public void RegisterComponent(ComponentDefinition component)
        {
            if (_components.Any(ent => ent.Alias.Equals(component.Alias)))
            {
                throw new ArgumentException(nameof(component), $"A component with the alias '{component.Alias}' was already registered.  Component aliases must be unique.");
            }

            _components.Add(component);
        }

        public IEnumerable<ComponentDefinition> ListComponents()
            => _components.AsEnumerable();

        public ComponentDefinition GetByAlias(string alias)
            => _components.FirstOrDefault(ent => string.Equals(ent.Alias, alias));
    }
}
