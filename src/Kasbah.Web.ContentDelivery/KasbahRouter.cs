using System.Linq;
using System.Threading.Tasks;
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
        readonly TypeMapper _typeMapper;

        public KasbahRouter(ILoggerFactory loggerFactory, ContentService contentService, TypeRegistry typeRegistry, KasbahWebApplication kasbahWebApplication, TypeMapper typeMapper)
        {
            _log = loggerFactory.CreateLogger<KasbahRouter>();
            _contentService = contentService;
            _typeRegistry = typeRegistry;
            _kasbahWebApplication = kasbahWebApplication;
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

                    routeData.Values["controller"] = "DefaultContent";
                    routeData.Values["action"] = "RenderContent";
                    routeData.Values["content"] = content;

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
