using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Delivery.Middleware
{
    public class NodeResolverMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly ContentService _contentService;

        public NodeResolverMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, ContentService contentService)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<NodeResolverMiddleware>();
            _contentService = contentService;
        }

        public async Task Invoke(HttpContext context)
        {
            var kasbahWebContext = context.GetKasbahWebContext();

            var site = kasbahWebContext.Site;
            if (site != null)
            {
                var requestPath = context.Request.Path.ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var contentPath = site.ContentRoot.Concat(requestPath);

                _log.LogDebug($"Trying to find content at: {string.Join(" / ", contentPath)}");
                var node = await _contentService.GetNodeByTaxonomy(contentPath);
                kasbahWebContext.Node = node;
            }

            await _next.Invoke(context);
        }
    }
}
