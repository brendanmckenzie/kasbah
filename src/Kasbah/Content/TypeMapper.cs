using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kasbah.Content.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Profiling;

namespace Kasbah.Content
{
    public class TypeMapper
    {
        readonly ILogger _log;
        readonly IContentProvider _contentProvider;
        readonly TypeRegistry _typeRegistry;
        readonly IEnumerable<ITypeHandler> _typeHandlers;
        readonly ProxyGenerator _generator;

        public TypeMapper(ILoggerFactory loggerFactory, IContentProvider contentProvider, IEnumerable<ITypeHandler> typeHandlers, TypeRegistry typeRegistry)
        {
            _log = loggerFactory.CreateLogger<TypeMapper>();
            _contentProvider = contentProvider;
            _typeHandlers = typeHandlers;
            _typeRegistry = typeRegistry;

            _generator = new ProxyGenerator();
        }

        public async Task<object> MapTypeAsync(string data, string typeName, Node node = null, long? version = null, TypeMapperContext context = null)
        {
            context = context ?? new TypeMapperContext();

            var dictData = string.IsNullOrEmpty(data) ? null : JsonConvert.DeserializeObject<IDictionary<string, object>>(data);

            return await MapTypeAsync(dictData, typeName, node, version, context);
        }

        public async Task<object> MapTypeAsync(IDictionary<string, object> data, string typeName, Node node = null, long? version = null, TypeMapperContext context = null)
        {
            using (MiniProfiler.Current.Step(nameof(MapTypeAsync)))
            {
                context = context ?? new TypeMapperContext();

                var type = Type.GetType(typeName);
                var typeInfo = type.GetTypeInfo();

                var options = new ProxyGenerationOptions();
                var caseConverter = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy();

                var ret = _generator.CreateClassProxy(type, options, new KasbahPropertyInterceptor(MapPropertyAsync, data, context));

                if (data != null)
                {
                    using (MiniProfiler.Current.Step("Mapping eager properties"))
                    {
                        var eagerLoadProperties = typeInfo.GetProperties()
                            .Where(prop => !typeof(IItem).IsAssignableFrom(prop.PropertyType) || (typeof(IItem).IsAssignableFrom(prop.PropertyType) && prop.GetMethod?.IsVirtual == false));

                        var values = await Task.WhenAll(eagerLoadProperties.Select(async prop =>
                        {
                            var key = prop.Name;

                            async Task<object> ExtractValue()
                            {
                                if (data.ContainsKey(key))
                                {
                                    return await MapPropertyAsync(data[key], prop, context);
                                }

                                var altKey = caseConverter.GetPropertyName(key, false);

                                if (data.ContainsKey(altKey))
                                {
                                    return await MapPropertyAsync(data[altKey], prop, context);
                                }

                                return null;
                            }

                            return new
                            {
                                Key = key,
                                Property = prop,
                                Value = await ExtractValue()
                            };
                        }));

                        foreach (var property in values.Where(ent => ent.Value != null))
                        {
                            property.Property.SetValue(ret, property.Value);
                        }
                    }
                }

                if (ret is Item item)
                {
                    if (item != null)
                    {
                        item.Node = node;
                        item.Id = node?.Id ?? Guid.Empty;

                        if (version.HasValue)
                        {
                            item.Version = version.Value;
                        }
                    }
                }

                return ret;
            }
        }

        public async Task<object> MapPropertyAsync(object source, PropertyInfo property, TypeMapperContext context)
        {
            if (source == null)
            {
                return null;
            }

            var sourceType = source.GetType();

            if (sourceType.Equals(property.PropertyType))
            {
                return source;
            }

            using (MiniProfiler.Current.Step($"{nameof(MapPropertyAsync)}('{property.Name}')"))
            {

                // Nested objects
                if (sourceType == typeof(JObject))
                {
                    if (typeof(IDictionary).IsAssignableFrom(property.PropertyType))
                    {
                        return (source as JObject).ToObject(property.PropertyType);
                    }
                    else
                    {
                        var dict = (source as JObject).ToObject<IDictionary<string, object>>();

                        return await MapTypeAsync(dict, property.PropertyType.AssemblyQualifiedName, context: context);
                    }
                }

                if (property.PropertyType.GetTypeInfo().IsEnum)
                {
                    try
                    {
                        return Enum.ToObject(property.PropertyType, int.Parse(source.ToString()));
                    }
                    catch (Exception)
                    {
                        return Enum.ToObject(property.PropertyType, 0);
                    }
                }

                // Linked objects
                if (_typeRegistry.GetType(property.PropertyType.AssemblyQualifiedName) != null)
                {
                    return await context.GetOrSetAsync($"{source}_linked", async () =>
                    {
                        return await MapLinkedObjectAsync(source, context);
                    });
                }

                // Linked objects (multiple)
                if (property.PropertyType.GenericTypeArguments.Any()
                    && (_typeRegistry.GetType(property.PropertyType.GenericTypeArguments.First().AssemblyQualifiedName) != null))
                {
                    var type = property.PropertyType.GenericTypeArguments.First();
                    var ret = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

                    var entries = await Task.WhenAll((source as JArray).ToArray()
                        .Select(ent => ent.ToString())
                        .Select(async id => await MapLinkedObjectAsync(id, context)));

                    foreach (var entry in entries)
                    {
                        ret.Add(entry);
                    }

                    return ret;
                }

                // Linked media
                var handlers = _typeHandlers.Where(ent => ent.CanConvert(property.PropertyType));
                if (handlers.Any())
                {
                    return await context.GetOrSetAsync($"{source}_media", async () =>
                        await handlers
                            .Select(async ent => await ent.ConvertAsync(source))
                            .FirstOrDefault(ent => ent != null));
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
        }

        async Task<object> MapLinkedObjectAsync(object source, TypeMapperContext context)
        {
            if (Guid.TryParse((string)source, out Guid id))
            {
                var node = await _contentProvider.GetNodeAsync(id);
                var type = _typeRegistry.GetType(node.Type);
                if (node.PublishedVersion.HasValue || !type.Fields.Any())
                {
                    try
                    {
                        var data = await _contentProvider.GetRawDataAsync(node.Id, node.PublishedVersion);

                        return await MapTypeAsync(data, node.Type, node, node.PublishedVersion, context);
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
    }
}
