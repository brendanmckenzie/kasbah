﻿using System;
using System.Collections.Generic;
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
        readonly SiteRegistry _siteRegistry;
        readonly TypeRegistry _typeRegistry;
        readonly KasbahWebApplication _kasbahWebApplication;
        readonly AnalyticsService _analyticsService;
        readonly TypeMapper _typeMapper;


        public KasbahRouter(ILoggerFactory loggerFactory, ContentService contentService, SiteRegistry siteRegistry, TypeRegistry typeRegistry, KasbahWebApplication kasbahWebApplication, AnalyticsService analyticsService, TypeMapper typeMapper)
        {
            _log = loggerFactory.CreateLogger<KasbahRouter>();
            _contentService = contentService;
            _siteRegistry = siteRegistry;
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

            var kasbahWebContext = new KasbahWebContext
            {
                WebApplication = _kasbahWebApplication,
                HttpContext = context.HttpContext,
                ContentService = _contentService,
                TypeRegistry = _typeRegistry,
                TypeMapper = _typeMapper,
                SiteRegistry = _siteRegistry
            };

            var profile = context.HttpContext.Items["user:profile"] as string;
            if (!string.IsNullOrEmpty(profile))
            {
                kasbahWebContext.Profile = new Guid(Convert.FromBase64String(profile));
            }

            await _analyticsService.TrackEventAsync(new AnalyticsEvent
            {
                Profile = kasbahWebContext.Profile ?? Guid.Empty,
                Type = "web:request",
                Source = "router",
                Data = new Dictionary<string, string>
                {
                    { "host", context.HttpContext.Request.Host.ToString() },
                    { "method", context.HttpContext.Request.Method },
                    { "path", context.HttpContext.Request.Path },
                    { "request", kasbahWebContext.RequestId.ToString() }
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
                    await _analyticsService.TrackEventAsync(new AnalyticsEvent
                    {
                        Profile = kasbahWebContext.Profile ?? Guid.Empty,
                        Type = "content:request",
                        Source = "router",
                        Data = new Dictionary<string, string>
                        {
                            { "node", node.Id.ToString() },
                            { "site", site.Alias },
                            { "version", node.PublishedVersion.Value.ToString() }
                        }
                    });

                    var type = _typeRegistry.GetType(node.Type);

                    routeData.Values["node"] = node;

                    var data = await _contentService.GetRawDataAsync(node.Id, node.PublishedVersion);
                    var content = await _typeMapper.MapTypeAsync(data, node.Type, node);

                    // TODO: apply patches

                    routeData.Values["content"] = content;

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
