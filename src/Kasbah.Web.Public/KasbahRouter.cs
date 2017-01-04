using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Public
{
    public class KasbahRouter : IRouter
    {
        readonly ILogger _log;
        readonly ContentService _contentService;
        readonly SiteRegistry _siteRegistry;


        public KasbahRouter(ILoggerFactory loggerFactory, ContentService contentService, SiteRegistry siteRegistry)
        {
            _log = loggerFactory.CreateLogger<KasbahRouter>();
            _contentService = contentService;
            _siteRegistry = siteRegistry;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
            => new VirtualPathData(this, context.HttpContext.Request.Path.Value);

        public async Task RouteAsync(RouteContext context)
        {
            var routeData = new RouteData(context.RouteData);

            _log.LogDebug($"Trying to match {context.HttpContext.Request.Host}.  Available sites: {string.Join(", ", _siteRegistry.ListSites().SelectMany(s => s.Domains))}");
            var site = _siteRegistry.GetSiteByDomain(context.HttpContext.Request.Host.ToString());
            if (site != null)
            {
                _log.LogDebug($"Site matched: {site.Alias}");

                routeData.Values["site"] = site;

                var requestPath = context.HttpContext.Request.Path.ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var contentPath = site.ContentRoot.Concat(requestPath);


                _log.LogDebug($"Trying to find content at: {string.Join(" / ", contentPath)}");
                var node = await _contentService.GetNodeByTaxonomy(contentPath);
                if (node != null && node.PublishedVersion.HasValue)
                {
                    routeData.Values["node"] = node;

                    var content = await _contentService.GetRawDataAsync(node.Id, node.PublishedVersion);

                    routeData.Values["content"] = content;

                    routeData.Values["controller"] = "DefaultContent";
                    routeData.Values["action"] = "RenderContent";

                    context.RouteData = routeData;
                }
            }
        }
    }
}
