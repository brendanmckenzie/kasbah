using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Prerendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kasbah.Web.Middleware.Delivery
{
    public class KasbahRouterMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly ISpaPrerenderer _prerenderer;
        readonly ComponentRegistry _componentRegistry;

        public KasbahRouterMiddleware(RequestDelegate next, ILogger<KasbahRouterMiddleware> log, ISpaPrerenderer prerenderer, ComponentRegistry componentRegistry)
        {
            _next = next;
            _log = log;
            _prerenderer = prerenderer;
            _componentRegistry = componentRegistry;
        }

        public async Task Invoke(HttpContext context)
        {
            var kasbahWebContext = context.GetKasbahWebContext();

            if (kasbahWebContext.Site != null)
            {
                var node = kasbahWebContext.Node;

                if (node != null && node.PublishedVersion.HasValue)
                {
                    var data = await kasbahWebContext.ContentService.GetRawDataAsync(node.Id, node.PublishedVersion);
                    var content = await kasbahWebContext.TypeMapper.MapTypeAsync(data, node.Type, node, node.PublishedVersion);

                    if (content is IPresentable presentable)
                    {
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

                                    return new
                                    {
                                        alias = ent.Control,
                                        model = await GetModelAsync(kasbahWebContext, properties, component)
                                    };
                                });

                            return new
                            {
                                key,
                                components = await Task.WhenAll(componentsAsync)
                            };
                        });

                        var renderData = await Task.WhenAll(renderDataAsync);

                        var model = new
                        {
                            node,
                            content,
                            site = kasbahWebContext.Site,
                            siteNode = kasbahWebContext.SiteNode,
                            components = renderData.ToDictionary(ent => ent.key, ent => ent.components)
                        };

                        if (context.Request.ContentType == "application/json")
                        {
                            var jsonSettings = new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            };
                            var json = JsonConvert.SerializeObject(model, jsonSettings);
                            await context.Response.WriteJsonAsync(json);
                        }
                        else
                        {
                            var result = await _prerenderer.RenderToString("wwwroot/dist/kasbah-server", customDataParameter: model);

                            if (!string.IsNullOrEmpty(result.RedirectUrl))
                            {
                                context.Response.Redirect(result.RedirectUrl, false);
                            }
                            else
                            {
                                await context.Response.WriteHtmlAsync($"<!DOCTYPE html>{result.Html}");
                            }
                        }
                    }
                    else
                    {
                        // handle situation where non-presentable node is trying to be routed
                    }

                    return;
                }
            }

            await _next.Invoke(context);
        }

        async Task<object> GetModelAsync(KasbahWebContext context, Component properties, ComponentDefinition component)
        {
            var asyncMethod = component.Control.GetMethod("GetModelAsync");
            if (asyncMethod != null)
            {
                var instance = Activator.CreateInstance(component.Control);

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
                    var instance = Activator.CreateInstance(component.Control);

                    return method.Invoke(instance, new object[] { properties, context });
                }
            }
        }
    }
}
