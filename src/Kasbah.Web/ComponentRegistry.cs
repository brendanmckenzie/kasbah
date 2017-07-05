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
            => _components.FirstOrDefault(ent => string.Equals(ent.Alias, alias));
    }
}
