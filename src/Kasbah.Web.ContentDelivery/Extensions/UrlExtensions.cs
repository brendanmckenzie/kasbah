using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.Extensions
{
    public static class UrlExtensions
    {
        public static async Task<PathString> ItemUrlAsync(this IUrlHelper urlHelper, Item item, bool absolute = false)
        {
            var kasbahWebContext = urlHelper.ActionContext.RouteData.Values["kasbahWebContext"] as KasbahWebContext;
            var node = await kasbahWebContext.ContentService.GetNodeAsync(item.Id);
            var site = kasbahWebContext.SiteRegistry.GetSiteByNode(node);

            var relativePath = string.Join("/", node.Taxonomy.Aliases.Skip(site.ContentRoot.Count()));

            if (absolute)
            {
                return $"{(site.UseSsl ? "https" : "http")}://{site.DefaultDomain}/{relativePath}";
            }

            return "/" + relativePath;
        }
    }
}
