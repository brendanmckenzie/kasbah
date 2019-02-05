using System;
using System.Linq;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.Models;

namespace Kasbah.Web
{
    public static class ComponentRegistryExtensions
    {
        public static void RegisterComponent<TComponent, TProperties>(this ComponentRegistry componentRegistry, Action<TypeDefinitionBuilder> configure = null)
            where TComponent : ComponentBase, new()
        {
            var instance = new TComponent();
            var type = typeof(TComponent);

            var builder = new TypeDefinitionBuilder<TProperties>(componentRegistry.PropertyMapper);

            configure?.Invoke(builder);

            var definition = new ComponentDefinition
            {
                Alias = instance.Alias,
                Control = type,
                Properties = builder.Build(),
                Placeholders = Enumerable.Empty<PlaceholderDefinition>()
            };

            componentRegistry.RegisterComponent(definition);
        }

        public static void RegisterComponent<TModel>(this ComponentRegistry componentRegistry)
            where TModel : ComponentBase, new()
        {
            var type = typeof(TModel);
            var instance = new TModel();

            // TODO: this is gross
            TypeDefinition GetProperties()
            {
                var method = type.GetMethod("GetModelAsync");
                if (method != null)
                {
                    // return type is Task<TModel>
                    var modelType = method.ReturnType.GenericTypeArguments.FirstOrDefault();

                    var builder = new TypeDefinitionBuilder(modelType, componentRegistry.PropertyMapper);

                    return builder.Build();
                }

                return null;
            }

            var definition = new ComponentDefinition
            {
                Alias = instance.Alias,
                Control = type,
                Placeholders = instance.Placeholders,
                Properties = GetProperties()
            };

            componentRegistry.RegisterComponent(definition);
        }
    }
}
