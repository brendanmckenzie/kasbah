﻿using System;
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
        readonly TypeRegistry _typeRegistry;
        readonly KasbahWebApplication _kasbahWebApplication;


        public KasbahRouter(ILoggerFactory loggerFactory, ContentService contentService, SiteRegistry siteRegistry, TypeRegistry typeRegistry, KasbahWebApplication kasbahWebApplication)
        {
            _log = loggerFactory.CreateLogger<KasbahRouter>();
            _contentService = contentService;
            _siteRegistry = siteRegistry;
            _typeRegistry = typeRegistry;
            _kasbahWebApplication = kasbahWebApplication;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
            => new VirtualPathData(this, context.HttpContext.Request.Path.Value);

        public async Task RouteAsync(RouteContext context)
        {
            var kasbahWebContext = new KasbahWebContext
            {
                WebApplication = _kasbahWebApplication,
                HttpContext = context.HttpContext,
                ContentService = _contentService
            };

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
