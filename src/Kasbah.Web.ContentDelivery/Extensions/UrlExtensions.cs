using System.Linq;
using Kasbah.Content.Models;
using Kasbah.Media.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.Extensions
{
    public static class UrlExtensions
    {
        public static PathString ItemUrl(this IUrlHelper urlHelper, Item item, bool absolute = false)
        {
            if (item == null) { return null; }

            var kasbahWebContext = urlHelper.ActionContext.RouteData.Values["kasbahWebContext"] as KasbahWebContext;
            var site = kasbahWebContext.SiteRegistry.GetSiteByNode(item.Node);

            var relativePath = string.Join("/", item.Node.Taxonomy.Aliases.Skip(site.ContentRoot.Count()));

            if (absolute)
            {
                return $"{(site.UseSsl ? "https" : "http")}://{site.DefaultDomain}/{relativePath}";
            }

            return "/" + relativePath;
        }

        public static PathString MediaUrlAsync(this IUrlHelper urlHelper, MediaItem mediaItem, bool absolute = false)
        {
            if (mediaItem == null) { return null; }

            var kasbahWebContext = urlHelper.ActionContext.RouteData.Values["kasbahWebContext"] as KasbahWebContext;
            // var site = kasbahWebContext.SiteRegistry.GetSiteByNode(item.Node);

            var relativePath = $"media/{mediaItem.Id}";

            if (absolute)
            {
                // return $"{(site.UseSsl ? "https" : "http")}://{site.DefaultDomain}/{relativePath}";
            }

            return "/" + relativePath;
        }
    }
}
