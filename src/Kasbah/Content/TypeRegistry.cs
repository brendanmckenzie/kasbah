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

        public TypeDefinition GetType(string type)
            => _types.SingleOrDefault(ent => ent.Alias == type);
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
            _fields = _typeInfo.GetProperties().Select(MapProperty).ToList();

            _displayName = type.Name;
        }

        public TypeDefinition Build()
        {
            return new TypeDefinition
            {
                DisplayName = _displayName,
                Alias = _type.FullName,
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
            UpdateField(fieldName, field => { field.DisplayName = displayName; });

            return this;
        }

        public TypeDefinitionBuilder FieldHelpText(string fieldName, string helpText)
        {
            UpdateField(fieldName, field => field.HelpText = helpText);

            return this;
        }

        public TypeDefinitionBuilder FieldCategory(string fieldName, string category)
        {
            UpdateField(fieldName, field => field.Category = category);

            return this;
        }

        void UpdateField(string fieldName, Action<TypeDefinition.Field> update)
        {
            var field = _fields.SingleOrDefault(ent => ent.Alias == fieldName);

            if (field == null) { throw new InvalidOperationException($"Unknown field: {fieldName}"); }

            update(field);
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