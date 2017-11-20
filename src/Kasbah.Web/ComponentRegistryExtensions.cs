using System;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web
{
    public static class ComponentRegistryExtensions
    {
        public static void RegisterComponent<TComponent, TProperties>(this ComponentRegistry componentRegistry, Action<TypeDefinitionBuilder> configure = null)
            where TComponent : ViewComponent
        {
            var type = typeof(TComponent);

            var builder = new TypeDefinitionBuilder<TProperties>(componentRegistry.PropertyMapper);

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
