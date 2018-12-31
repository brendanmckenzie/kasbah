using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Web.Models;
using Kasbah.Web.Models.Delivery;
using Microsoft.AspNetCore.Http;
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

        public async Task Invoke(HttpContext context)
        {
            var kasbahWebContext = context.GetKasbahWebContext();

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
                    var data = await kasbahWebContext.ContentService.GetRawDataAsync(node.Id, node.PublishedVersion);
                    var content = await kasbahWebContext.TypeMapper.MapTypeAsync(data, node.Type, node, node.PublishedVersion);

                    if (content is IPresentable presentable)
                    {
                        // TODO: process Control tree for `BodyControl` and `HeadControl`

                        var instanceComponents = presentable.Components;
                        var staticComponents = await presentable.ListStaticComponentsAsync(kasbahWebContext) ?? new ComponentCollection();
                        var allComponents = instanceComponents.Concat(staticComponents)
                            .ToDictionary(ent => ent.Key, ent => ent.Value);

                        var renderDataAsync = allComponents.Keys.AsParallel().Select(async key =>
                        {
                            var componentList = allComponents[key];
                            var componentsAsync = componentList
                                .AsParallel()
                                .AsOrdered()
                                .Select(async ent =>
                                {
                                    var component = _componentRegistry.GetByAlias(ent.Control);
                                    var properties = ent.Properties == null ? null : await kasbahWebContext.TypeMapper.MapTypeAsync(ent.Properties, component.Properties.Alias) as Component;

                                    return new RenderModel.Component
                                    {
                                        Alias = ent.Control,
                                        Model = await GetModelAsync(kasbahWebContext, properties, component)
                                    };
                                });

                            return new
                            {
                                key,
                                components = await Task.WhenAll(componentsAsync)
                            };
                        });

                        var renderData = await Task.WhenAll(renderDataAsync);

                        var model = new RenderModel
                        {
                            TraceIdentifier = context.TraceIdentifier,
                            Node = node,
                            Content = content,
                            Site = kasbahWebContext.Site,
                            SiteNode = kasbahWebContext.SiteNode,
                            Layout = presentable.Layout,
                            Components = new RenderModel.ComponentMap(renderData.ToDictionary(ent => ent.key, ent => ent.components.AsEnumerable()))
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

        async Task<object> GetModelAsync(KasbahWebContext context, Component properties, ComponentDefinition component)
        {
            var asyncMethod = component.Control.GetMethod("GetModelAsync");
            if (asyncMethod != null)
            {
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, component.Control);

                var task = (Task<object>)asyncMethod.Invoke(instance, new object[] { properties, context });

                return await task;
            }
            else
            {
                var method = component.Control.GetMethod("GetModel");
                if (method == null)
                {
                    return properties;
                }
                else
                {
                    var instance = ActivatorUtilities.CreateInstance(_serviceProvider, component.Control);

                    return method.Invoke(instance, new object[] { properties, context });
                }
            }
        }
    }
}
