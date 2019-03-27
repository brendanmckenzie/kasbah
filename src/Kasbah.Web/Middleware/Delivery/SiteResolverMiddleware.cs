using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace Kasbah.Web.Middleware.Delivery
{
    public class SiteResolverMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly SiteRegistry _siteRegistry;
        readonly ContentService _contentService;

        public SiteResolverMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, SiteRegistry siteRegistry, ContentService contentService)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<SiteResolverMiddleware>();
            _siteRegistry = siteRegistry;
            _contentService = contentService;
        }

        public async Task Invoke(HttpContext context)
        {
            using (MiniProfiler.Current.Step(nameof(SiteResolverMiddleware)))
            {
                // TODO: use below for finding site
                // var sites = await _contentService.GetNodesByType("Site");
                _log.LogDebug($"Trying to match {context.Request.Host}.  Available sites: {string.Join(", ", _siteRegistry.ListSites().SelectMany(s => s.Hostnames ?? Enumerable.Empty<string>()))}");

                var kasbahWebContext = context.GetKasbahWebContext();

                kasbahWebContext.Site = _siteRegistry.GetSiteByDomain(context.Request.Host);
                if (kasbahWebContext.Site != null)
                {
                    _log.LogDebug($"Site matched: {kasbahWebContext.Site.Alias}");

                    kasbahWebContext.SiteNode = await _contentService.GetNodeByTaxonomy(kasbahWebContext.Site.ContentRoot);
                }
            }

            await _next.Invoke(context);
        }
    }
}
