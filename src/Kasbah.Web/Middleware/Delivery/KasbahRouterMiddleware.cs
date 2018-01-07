using System.Linq;
using System.Threading.Tasks;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Prerendering;
using Microsoft.Extensions.Logging;

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
                        var renderDataAsync = presentable.Components.Keys.AsParallel().Select(async key =>
                        {
                            var componentsAsync = presentable.Components[key].AsParallel().Select(async ent =>
                            {
                                var component = _componentRegistry.GetByAlias(ent.Control);
                                var properties = await kasbahWebContext.TypeMapper.MapTypeAsync(ent.Properties, component.Properties.Alias) as Component;

                                return new
                                {
                                    alias = ent.Control,
                                    properties,
                                    model = properties.GetModel(kasbahWebContext)
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
                            site = kasbahWebContext.Site,
                            siteNode = kasbahWebContext.SiteNode,
                            components = renderData.ToDictionary(ent => ent.key, ent => ent.components)
                        };

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
                    else
                    {
                        // handle situation where non-presentable node is trying to be routed
                    }

                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
