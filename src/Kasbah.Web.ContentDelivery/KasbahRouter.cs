using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.ContentDelivery
{
    public class KasbahRouter : IRouter
    {
        readonly ILogger _log;
        readonly ContentService _contentService;
        readonly SiteRegistry _siteRegistry;
        readonly TypeRegistry _typeRegistry;
        readonly KasbahWebApplication _kasbahWebApplication;
        readonly AnalyticsService _analyticsService;


        public KasbahRouter(ILoggerFactory loggerFactory, ContentService contentService, SiteRegistry siteRegistry, TypeRegistry typeRegistry, KasbahWebApplication kasbahWebApplication, AnalyticsService analyticsService)
        {
            _log = loggerFactory.CreateLogger<KasbahRouter>();
            _contentService = contentService;
            _siteRegistry = siteRegistry;
            _typeRegistry = typeRegistry;
            _kasbahWebApplication = kasbahWebApplication;
            _analyticsService = analyticsService;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
            => new VirtualPathData(this, context.HttpContext.Request.Path.Value);

        public async Task RouteAsync(RouteContext context)
        {
            _kasbahWebApplication.RequestsTotal++;
            Jobs.HeartbeatJob.RequestsLatest++;

            var kasbahWebContext = new KasbahWebContext
            {
                WebApplication = _kasbahWebApplication,
                HttpContext = context.HttpContext,
                ContentService = _contentService,
                TypeRegistry = _typeRegistry
            };

            await _analyticsService.TrackEvent(new AnalyticsEvent
            {
                Type = "request",
                Source = nameof(KasbahRouter),
                Data = new Dictionary<string, object>
                {
                    { "host", context.HttpContext.Request.Host },
                    { "method", context.HttpContext.Request.Method },
                    { "path", context.HttpContext.Request.Path },
                    { "request", kasbahWebContext.RequestId }
                }
            });

            var routeData = new RouteData(context.RouteData);

            routeData.Values["kasbahWebContext"] = kasbahWebContext;

            _log.LogDebug($"Trying to match {context.HttpContext.Request.Host}.  Available sites: {string.Join(", ", _siteRegistry.ListSites().SelectMany(s => s.Domains))}");
            var site = _siteRegistry.GetSiteByDomain(context.HttpContext.Request.Host.ToString());
            if (site != null)
            {
                kasbahWebContext.Site = site;
                _log.LogDebug($"Site matched: {site.Alias}");

                routeData.Values["site"] = site;

                var requestPath = context.HttpContext.Request.Path.ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var contentPath = site.ContentRoot.Concat(requestPath);

                _log.LogDebug($"Trying to find content at: {string.Join(" / ", contentPath)}");
                var node = await _contentService.GetNodeByTaxonomy(contentPath);
                kasbahWebContext.Node = node;
                if (node != null && node.PublishedVersion.HasValue)
                {
                    await _analyticsService.TrackEvent(new AnalyticsEvent
                    {
                        Type = "request:content",
                        Source = nameof(KasbahRouter),
                        Data = new Dictionary<string, object>
                        {
                            { "node", node.Id },
                            { "site", site.Alias },
                            { "version", node.PublishedVersion }
                        }
                    });

                    var type = _typeRegistry.GetType(node.Type);

                    routeData.Values["node"] = node;

                    var content = await _contentService.GetTypedDataAsync(node.Id, node.PublishedVersion);

                    routeData.Values["content"] = content;

                    routeData.Values["controller"] = "DefaultContent";
                    routeData.Values["action"] = "RenderContent";

                    foreach (var key in type.Options.Keys)
                    {
                        routeData.Values[key] = type.Options[key];
                    }

                    context.RouteData = routeData;
                }
            }
        }
    }
}
