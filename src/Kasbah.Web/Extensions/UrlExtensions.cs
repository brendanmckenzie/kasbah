using System.Linq;
using Kasbah.Content.Models;
using Kasbah.Media.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web
{
    public static class UrlExtensions
    {
        public static PathString ItemUrl(this KasbahWebContext kasbahWebContext, Item item, bool absolute = false)
        {
            if (item == null)
            {
                return null;
            }

            var site = kasbahWebContext.Site;

            var relativePath = string.Join("/", item.Node.Taxonomy.Aliases.Skip(site.ContentRoot.Count()));

            if (absolute)
            {
                return $"{(site.UseSsl ? "https" : "http")}://{site.DefaultDomain}/{relativePath}";
            }

            return "/" + relativePath;
        }

        public static PathString ItemUrl(this IUrlHelper urlHelper, Item item, bool absolute = false)
        {
            var kasbahWebContext = urlHelper.ActionContext.HttpContext.GetKasbahWebContext();

            return kasbahWebContext.ItemUrl(item, absolute);
        }

        public static PathString MediaUrlAsync(this IUrlHelper urlHelper, MediaItem mediaItem, bool absolute = false)
        {
            if (mediaItem == null)
            {
                return null;
            }

            var kasbahWebContext = urlHelper.ActionContext.HttpContext.GetKasbahWebContext();

            var site = kasbahWebContext.Site;
            var relativePath = $"media/{mediaItem.Id}";

            if (absolute)
            {
                return $"{(site.UseSsl ? "https" : "http")}://{site.DefaultDomain}/{relativePath}";
            }

            return "/" + relativePath;
        }
    }
}
