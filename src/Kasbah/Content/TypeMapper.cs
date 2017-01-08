using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Kasbah.Content
{
    public class TypeMapper
    {
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;

        public TypeMapper(ContentService contentService, TypeRegistry typeRegistry)
        {
            _contentService = contentService;
            _typeRegistry = typeRegistry;
        }

        public async Task<object> MapTypeAsync(IDictionary<string, object> data, string typeName)
        {
            var type = Type.GetType(typeName);
            var typeInfo = type.GetTypeInfo();

            var ret = Activator.CreateInstance(type);
            foreach (var property in typeInfo.GetProperties())
            {
                var key = property.Name;
                if (data.ContainsKey(key))
                {
                    var source = data[key];
                    var dest = await MapPropertyAsync(source, property);

                    property.SetValue(ret, dest);
                }
            }

            return ret;
        }

        public async Task<object> MapPropertyAsync(object source, PropertyInfo property)
        {
            if (source == null) { return null; }

            var sourceType = source.GetType();

            // Nested objects
            if (sourceType == typeof(JObject))
            {
                var dict = (source as JObject).ToObject<IDictionary<string, object>>();

                return await MapTypeAsync(dict, property.PropertyType.AssemblyQualifiedName);
            }

            // Linked objects
            if (_typeRegistry.GetType(property.PropertyType.AssemblyQualifiedName) != null)
            {
                Guid id;
                if (Guid.TryParse((string)source, out id))
                {
                    var node = await _contentService.GetNodeAsync(id);
                    if (node.PublishedVersion.HasValue)
                    {
                        return await _contentService.GetTypedDataAsync(id, node.PublishedVersion.Value);
                    }
                }
            }

            // TODO: array of linked objects

            try
            {
                return Convert.ChangeType(source, property.PropertyType);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
    }
}