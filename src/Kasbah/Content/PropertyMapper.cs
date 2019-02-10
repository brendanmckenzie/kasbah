using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Kasbah.Content.Attributes;
using Kasbah.Content.Models;

namespace Kasbah.Content
{
    public class PropertyMapper
    {
        const string DefaultEditor = "text";

        static IDictionary<Type, string> _knownTypeEditors = new Dictionary<Type, string>
        {
            { typeof(string), "text" },
            { typeof(DateTime), "date" },
            { typeof(Enum), "enum" },
            { typeof(bool), "boolean" },
        };

        static IEnumerable<Type> _basicEditors = _knownTypeEditors.Keys.Concat(new[]
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(Enum)
        }).Distinct();

        public TypeDefinition.Field MapProperty(PropertyInfo property)
        {
            var typeInfo = property.PropertyType.GetTypeInfo();

            var editor = _knownTypeEditors.ContainsKey(property.PropertyType) ? _knownTypeEditors[property.PropertyType] : DefaultEditor;
            var ret = new TypeDefinition.Field
            {
                DisplayName = property.Name,
                Alias = property.Name,
                Type = property.PropertyType.Name,
                Editor = editor
            };

            var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttribute != null)
            {
                ret.DisplayName = displayNameAttribute.DisplayName;
            }

            var editorAttribute = property.GetCustomAttribute<FieldEditorAttribute>(true);
            if (editorAttribute != null)
            {
                ret.Editor = editorAttribute.Editor;
            }
            else if (typeof(Item).GetTypeInfo().IsAssignableFrom(property.PropertyType))
            {
                ret.Editor = "nodePicker";
                ret.Type = property.PropertyType.AssemblyQualifiedName;
            }
            else if (property.PropertyType.GetTypeInfo().IsEnum)
            {
                ret.Editor = "enum";
                ret.Options["enumType"] = property.PropertyType.GetTypeInfo().Name;
                ret.Options["enum"] = Enum.GetValues(property.PropertyType)
                                          .OfType<object>()
                                          .Select(value => new
                                          {
                                              Name = Enum.GetName(property.PropertyType, value),
                                              Value = value,
                                          }).ToList();
            }
            else if (typeof(IEnumerable<Item>).GetTypeInfo().IsAssignableFrom(property.PropertyType))
            {
                ret.Editor = "nodePickerMulti";
                ret.Type = property.PropertyType.GenericTypeArguments.First().AssemblyQualifiedName;
            }
            else if (!_basicEditors.Contains(property.PropertyType))
            {
                ret.Editor = "nested";
                ret.Options["fields"] = typeInfo.GetProperties().Select(MapProperty).ToList(); // TODO: guard against recurrsion
            }

            return ret;
        }
    }
}
