using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kasbah.Content.Models;
using Kasbah.Media;
using Kasbah.Media.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Kasbah.Content
{
    public class TypeMapper
    {
        readonly ILogger _log;
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;
        readonly MediaService _mediaService;
        readonly ProxyGenerator _generator;

        public TypeMapper(ILoggerFactory loggerFactory, ContentService contentService, MediaService mediaService, TypeRegistry typeRegistry)
        {
            _log = loggerFactory.CreateLogger<TypeMapper>();
            _contentService = contentService;
            _mediaService = mediaService;
            _typeRegistry = typeRegistry;

            _generator = new ProxyGenerator();
        }

        public async Task<object> MapTypeAsync(IDictionary<string, object> data, string typeName, Node node = null, int? version = null)
        {
            var type = Type.GetType(typeName);
            var typeInfo = type.GetTypeInfo();

            var options = new ProxyGenerationOptions(new MethodSelectorHook());

            var ret = _generator.CreateClassProxy(type, options, new KasbahPropertyInterceptor(MapPropertyAsync, data));
            if (data != null)
            {
                var eagerLoadProperties = typeInfo.GetProperties().Where(prop => prop.GetMethod?.IsVirtual == false);
                var values = await Task.WhenAll(eagerLoadProperties.Select(async prop =>
                {
                    var key = prop.Name;
                    return new
                    {
                        Key = key,
                        Property = prop,
                        Value = data.ContainsKey(key) ? await MapPropertyAsync(data[key], prop) : null
                    };
                }));

                foreach (var property in values.Where(ent => ent.Value != null))
                {
                    property.Property.SetValue(ret, property.Value);
                }
            }

            if (ret is Item)
            {
                var item = ret as Item;

                if (item != null)
                {
                    item.Node = node;
                    item.Id = node?.Id ?? Guid.Empty;
                }

                if (version.HasValue)
                {
                    (ret as Item).Version = version.Value;
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
            if (property.PropertyType.GenericTypeArguments.Any()
                && (_typeRegistry.GetType(property.PropertyType.GenericTypeArguments.First().AssemblyQualifiedName) != null))
            {
                var type = property.PropertyType.GenericTypeArguments.First();
                var ret = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

                var entries = await Task.WhenAll((source as JArray).ToArray()
                    .Select(ent => ent.ToString())
                    .Select(async id => await MapLinkedObjectAsync(id)));

                foreach (var entry in entries)
                {
                    ret.Add(entry);
                }

                return ret;
            }

            // Linked media
            if (typeof(MediaItem).GetTypeInfo().IsAssignableFrom(property.PropertyType))
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
                    try
                    {
                        var data = await _contentService.GetRawDataAsync(node.Id, node.PublishedVersion);
                        return await MapTypeAsync(data, node.Type, node);
                    }
                    catch
                    {
                        _log.LogDebug($"Failed to map linked object {id}");
                        return null;
                    }
                }
            }

            return null;
        }

        async Task<object> MapLinkedMediaAsync(object source)
        {
            Guid id;
            if (Guid.TryParse((string)source, out id))
            {
                try
                {
                    return await _mediaService.GetMediaItemAsync(id);
                }
                catch
                {
                    _log.LogDebug($"Failed to map linked media {id}");
                    return null;
                }
            }

            return null;
        }

        delegate Task<object> MapPropertyDelegate(object source, PropertyInfo property);

        class KasbahPropertyInterceptor : IInterceptor
        {
            readonly MapPropertyDelegate _mapProperty;
            readonly IDictionary<string, object> _data;
            readonly IDictionary<string, object> _cache;

            public KasbahPropertyInterceptor(MapPropertyDelegate mapProperty, IDictionary<string, object> data)
            {
                _mapProperty = mapProperty;
                _data = data;
                _cache = new Dictionary<string, object>();
            }

            public void Intercept(IInvocation invocation)
            {
                var propertyName = invocation.Method.Name.Substring(4);

                if (!_cache.ContainsKey(propertyName))
                {
                    var property = invocation.TargetType.GetProperty(propertyName);

                    if (_data.ContainsKey(propertyName))
                    {
                        _cache.Add(propertyName, _mapProperty.Invoke(_data[propertyName], property).Result);
                    }
                    else
                    {
                        _cache.Add(propertyName, null);
                    }
                }

                invocation.ReturnValue = _cache[propertyName];
            }
        }

        class MethodSelectorHook : IProxyGenerationHook
        {
            public void MethodsInspected()
            {
            }

            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
            {
            }

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
                => methodInfo.Name.StartsWith("get_");

        }
    }
}
