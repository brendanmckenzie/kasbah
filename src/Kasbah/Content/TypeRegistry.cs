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
            => _types.Add(type);

        public IEnumerable<TypeDefinition> ListTypes()
            => _types.AsEnumerable();
    }

    public static class TypeRegistryExtensions
    {
        public static TypeRegistry Register<T>(this TypeRegistry registry, Action<TypeDefinitionBuilder> configure = null)
        {
            var builder = new TypeDefinitionBuilder<T>();

            configure?.Invoke(builder);

            registry.RegisterType(builder.Build());

            return registry;
        }
    }

    public class TypeDefinitionBuilder
    {
        readonly Type _type;
        readonly TypeInfo _typeInfo;
        readonly IEnumerable<TypeDefinition.Field> _fields;
        string _displayName;

        public TypeDefinitionBuilder(Type type)
        {
            _type = type;
            _typeInfo = type.GetTypeInfo();
            _fields = _typeInfo.GetProperties().Select(MapProperty);

            _displayName = type.Name;
        }

        public TypeDefinition Build()
        {
            return new TypeDefinition
            {
                DisplayName = _displayName,
                Type = _type,
                Fields = _fields
            };
        }

        public TypeDefinitionBuilder DisplayName(string displayName)
        {
            _displayName = displayName;

            return this;
        }

        public TypeDefinitionBuilder FieldDisplayName(string fieldName, string displayName)
        {
            UpdateField(fieldName, field => field.DisplayName = displayName);

            return this;
        }

        public TypeDefinitionBuilder FieldHelpText(string fieldName, string helpText)
        {
            UpdateField(fieldName, field => field.HelpText = helpText);

            return this;
        }

        void UpdateField(string fieldName, Action<TypeDefinition.Field> update)
        {
            var field = _fields.SingleOrDefault(ent => ent.Alias == fieldName);
            if (field != null)
            {
                update(field);
            }
        }

        static TypeDefinition.Field MapProperty(PropertyInfo property)
        {
            return new TypeDefinition.Field
            {
                DisplayName = property.Name,
                Alias = property.Name,
                Type = property.PropertyType.Name
            };
        }
    }

    public class TypeDefinitionBuilder<T> : TypeDefinitionBuilder
    {
        public TypeDefinitionBuilder() : base(typeof(T)) { }
    }
}