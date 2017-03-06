using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.ContentDelivery.Middleware
{
    public class SiteResolverMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly SiteRegistry _siteRegistry;

        public SiteResolverMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, SiteRegistry siteRegistry)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<SiteResolverMiddleware>();
            _siteRegistry = siteRegistry;
        }

        public async Task Invoke(HttpContext context)
        {
            _log.LogDebug($"Trying to match {context.Request.Host}.  Available sites: {string.Join(", ", _siteRegistry.ListSites().SelectMany(s => s.Domains))}");

            var kasbahWebContext = context.Items["kasbahWebContext"] as KasbahWebContext;

            kasbahWebContext.Site = _siteRegistry.GetSiteByDomain(context.Request.Host.ToString());
            if (kasbahWebContext.Site != null)
            {
                _log.LogDebug($"Site matched: {kasbahWebContext.Site.Alias}");
            }

            await _next.Invoke(context);
        }
    }
}
