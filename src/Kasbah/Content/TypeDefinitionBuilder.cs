using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kasbah.Content.Attributes;
using Kasbah.Content.Models;

namespace Kasbah.Content
{
    public class TypeDefinitionBuilder
    {
        static IEnumerable<string> _reservedFields = new[]
        {
            nameof(Item.Version),
            nameof(Item.Id),
            nameof(Item.Node)
        };

        readonly Type _type;
        readonly TypeInfo _typeInfo;
        readonly IEnumerable<TypeDefinition.Field> _fields;
        readonly IDictionary<string, object> _options;
        string _displayName;
        string _icon;
        string _iconColour;

        public TypeDefinitionBuilder(Type type, PropertyMapper propertyMapper)
        {
            _type = type;
            _typeInfo = type.GetTypeInfo();
            _fields = _typeInfo.GetProperties()
                .Where(ent => !_reservedFields.Contains(ent.Name))
                .Select(propertyMapper.MapProperty)
                .ToList();
            _options = new Dictionary<string, object>();

            var iconAttribute = _typeInfo.GetCustomAttribute<IconAttribute>();
            _icon = iconAttribute?.Icon;
            _iconColour = iconAttribute?.Colour;

            _displayName = type.Name;
        }

        public TypeDefinition Build()
        {
            return new TypeDefinition
            {
                DisplayName = _displayName,
                Alias = _type.AssemblyQualifiedName,
                Options = _options,
                Fields = _fields,
                Icon = _icon,
                IconColour = _iconColour
            };
        }

        public TypeDefinitionBuilder DisplayName(string displayName)
        {
            _displayName = displayName;

            return this;
        }

        public TypeDefinitionBuilder SetOption(string key, string value)
        {
            _options[key] = value;

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

        public TypeDefinitionBuilder FieldEditor(string fieldName, string editor)
        {
            UpdateField(fieldName, field => field.Editor = editor);

            return this;
        }

        void UpdateField(string fieldName, Action<TypeDefinition.Field> update)
        {
            var field = _fields.SingleOrDefault(ent => ent.Alias == fieldName);

            if (field == null)
            {
                throw new InvalidOperationException($"Unknown field: {fieldName}");
            }

            update(field);
        }
    }
}
