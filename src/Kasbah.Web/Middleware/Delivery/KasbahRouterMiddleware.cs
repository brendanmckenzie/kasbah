using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.Models;
using Kasbah.Web.Models.Delivery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kasbah.Web.Middleware.Delivery
{
    public class KasbahRouterMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly ComponentRegistry _componentRegistry;
        readonly IMemoryCache _cache;
        readonly IServiceProvider _serviceProvider;

        public KasbahRouterMiddleware(RequestDelegate next, ILogger<KasbahRouterMiddleware> log, ComponentRegistry componentRegistry, IMemoryCache cache, IServiceProvider serviceProvider)
        {
            _next = next;
            _log = log;
            _componentRegistry = componentRegistry;
            _cache = cache;
            _serviceProvider = serviceProvider;
        }

        // TODO: this method is gross.
        public async Task Invoke(HttpContext context)
        {
            var kasbahWebContext = context.GetKasbahWebContext();

            // Load the model from the cache if this is a subsequent request
            if (context.Request.Query.TryGetValue("ti", out var traceIdentifier))
            {
                var model = _cache.Get($"model:{traceIdentifier.First()}");

                context.Items["kasbah:model"] = model;
            }

            if (context.Items["kasbah:model"] == null && kasbahWebContext.Site != null)
            {
                var node = kasbahWebContext.Node;

                if (node != null && node.PublishedVersion.HasValue)
                {
                    var content = await GetContent(node, kasbahWebContext.TypeMapper, kasbahWebContext.ContentService);

                    if (content is IPresentable presentable)
                    {
                        var typeMapperContext = new TypeMapperContext();

                        async Task<ControlRenderModel> ControlToRenderModel(Control control)
                        {
                            if (control == null || string.IsNullOrEmpty(control.Alias))
                            {
                                return null;
                            }

                            var component = _componentRegistry.GetByAlias(control.Alias);

                            if (component == null)
                            {
                                _log.LogInformation($"Referenced component '{control.Alias}' not found");

                                return null;
                            }

                            async Task<object> ExtractProperties()
                            {
                                if (control.Model == null)
                                {
                                    return null;
                                }

                                var dict = control.Model.ToObject<IDictionary<string, object>>();

                                return await kasbahWebContext.TypeMapper.MapTypeAsync(dict, component.Properties.Alias, kasbahWebContext.Node, kasbahWebContext.Node.PublishedVersion, typeMapperContext);
                            }

                            var properties = await ExtractProperties();

                            var controlModel = await GetModelAsync(kasbahWebContext, properties, component, presentable);

                            var placeholderTasks = (control.Placeholders ?? new PlaceholderCollection()).Select(async ent => new KeyValuePair<string, IEnumerable<object>>(ent.Key, await Task.WhenAll(ent.Value.Select(ControlToRenderModel))));

                            var placeholders = await Task.WhenAll(placeholderTasks);

                            return new ControlRenderModel
                            {
                                Component = control.Alias,
                                Model = controlModel,
                                Controls = placeholders.ToDictionary(ent => ent.Key, ent => ent.Value)
                            };
                        }

                        var layout = await ControlToRenderModel(presentable.Layout);

                        var model = new RenderModel
                        {
                            TraceIdentifier = context.TraceIdentifier,
                            Node = node,
                            Site = kasbahWebContext.Site,
                            SiteNode = kasbahWebContext.SiteNode,
                            Layout = layout
                        };

                        context.Items["kasbah:model"] = model;

                        _cache.Set($"model:{context.TraceIdentifier}", model, TimeSpan.FromMinutes(5));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        if (context.Request.Headers.TryGetValue("Accept", out var accept) && accept.Contains("application/json"))
                        {
                            context.Response.Headers.Add("Content-Type", "application/json");
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Requested content cannot be rendered" }));
                        }
                        else
                        {
                            await context.Response.WriteAsync("Requested content cannot be rendered");
                        }

                        return;
                    }
                }
            }

            await _next.Invoke(context);
        }

        async Task<object> GetModelAsync(KasbahWebContext context, object properties, ComponentDefinition component, IPresentable content)
        {
            var asyncMethod = component.Control.GetMethod(nameof(ComponentBase<object>.GetModelAsync));
            if (asyncMethod == null)
            {
                return null;
            }

            dynamic instance = ActivatorUtilities.CreateInstance(_serviceProvider, component.Control);
            try
            {
                var task = (Task)asyncMethod.Invoke(instance, new object[] { context, properties, content });

                await task.ConfigureAwait(false);

                return (object)((dynamic)task).Result;
            }
            finally
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        async Task<object> GetContent(Node node, TypeMapper typeMapper, ContentService contentService)
        {
            // TODO: this should be in `ContentService`
            // TODO: apply patches
            var data = await contentService.GetRawDataAsync(node.Id, node.PublishedVersion);

            // var dataAsOperations = JsonConvert.DeserializeObject<List<Operation>>(data);
            // var contractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            // var patch = new JsonPatchDocument(dataAsOperations, contractResolver);
            var content = await typeMapper.MapTypeAsync(data, node.Type, node, node.PublishedVersion);

            return content;
        }
    }
}
