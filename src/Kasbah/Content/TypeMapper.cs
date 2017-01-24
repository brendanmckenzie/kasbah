using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Media;
using Kasbah.Media.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kasbah.Content
{
    public class TypeMapper
    {
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;
        readonly MediaService _mediaService;

        public TypeMapper(ContentService contentService, MediaService mediaService, TypeRegistry typeRegistry)
        {
            _contentService = contentService;
            _mediaService = mediaService;
            _typeRegistry = typeRegistry;
        }

        public async Task<object> MapTypeAsync(IDictionary<string, object> data, string typeName)
        {
            var type = Type.GetType(typeName);
            var typeInfo = type.GetTypeInfo();

            var ret = Activator.CreateInstance(type);
            if (data != null)
            {
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
                return await MapLinkedObjectAsync(source);
            }

            // Linked objects (multiple)
            if (property.PropertyType.IsGenericParameter && (_typeRegistry.GetType(property.PropertyType.GenericTypeArguments[0].AssemblyQualifiedName) != null))
            {
                var typeName = property.PropertyType.GenericTypeArguments[0].AssemblyQualifiedName;

                return Task.WhenAll((source as IEnumerable<string>).Select(async ent => await MapLinkedObjectAsync(ent)));
            }

            // Linked media
            if (typeof(MediaItem).IsAssignableFrom(property.PropertyType))
            {
                return await MapLinkedMediaAsync(source);
            }

            try
            {
                return Convert.ChangeType(source, property.PropertyType);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        async Task<object> MapLinkedObjectAsync(object source)
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

            return null;
        }

        async Task<object> MapLinkedMediaAsync(object source)
        {
            Guid id;
            if (Guid.TryParse((string)source, out id))
            {
                return await _mediaService.GetMediaItemAsync(id);
            }

            return null;
        }
    }
}
