using System.Linq;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Kasbah.Content;
using Kasbah.Web.ContentDelivery.Models;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.ContentDelivery
{
    public class KasbahRouter : IRouter
    {
        readonly ILogger _log;
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;
        readonly KasbahWebApplication _kasbahWebApplication;
        readonly AnalyticsService _analyticsService;
        readonly TypeMapper _typeMapper;


        public KasbahRouter(ILoggerFactory loggerFactory, ContentService contentService, TypeRegistry typeRegistry, KasbahWebApplication kasbahWebApplication, AnalyticsService analyticsService, TypeMapper typeMapper)
        {
            _log = loggerFactory.CreateLogger<KasbahRouter>();
            _contentService = contentService;
            _typeRegistry = typeRegistry;
            _kasbahWebApplication = kasbahWebApplication;
            _analyticsService = analyticsService;
            _typeMapper = typeMapper;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
            => new VirtualPathData(this, context.HttpContext.Request.Path.Value);

        public async Task RouteAsync(RouteContext context)
        {
            _kasbahWebApplication.RequestsTotal++;
            Jobs.HeartbeatJob.RequestsLatest++;

            var routeData = new RouteData(context.RouteData);

            var kasbahWebContext = (routeData.Values["kasbahWebContext"] = context.HttpContext.Items["kasbahWebContext"]) as KasbahWebContext;

            if (kasbahWebContext.Site != null)
            {
                routeData.Values["site"] = kasbahWebContext.Site;

                var node = kasbahWebContext.Node;

                if (node != null && node.PublishedVersion.HasValue)
                {
                    var type = _typeRegistry.GetType(node.Type);

                    routeData.Values["node"] = node;

                    var data = await _contentService.GetRawDataAsync(node.Id, node.PublishedVersion);
                    var content = await _typeMapper.MapTypeAsync(data, node.Type, node);

                    if (content is IBiasedContent)
                    {
                        var biasedContent = content as IBiasedContent;

                        if (biasedContent.Bias != null)
                        {
                            await Task.WhenAll(biasedContent.Bias.Select(async ent => await _analyticsService.TriggerBiasAsync(kasbahWebContext.Profile, ent.Key, ent.Value)));
                        }
                    }

                    if (content is IPatchedContent)
                    {
                        routeData.Values["content"] = await _analyticsService.PatchContentAsync(kasbahWebContext.Profile, node, content, type);
                    }
                    else
                    {
                        routeData.Values["content"] = content;
                    }

                    routeData.Values["controller"] = "DefaultContent";
                    routeData.Values["action"] = "RenderContent";

                    foreach (var key in type.Options.Keys)
                    {
                        routeData.Values[key] = type.Options[key];
                    }

                    var resolvedView = (content as IViewResolver)?.GetView(kasbahWebContext);
                    if (!string.IsNullOrEmpty(resolvedView))
                    {
                        routeData.Values["view"] = resolvedView;
                    }

                    context.RouteData = routeData;
                }
            }
        }
    }
}
