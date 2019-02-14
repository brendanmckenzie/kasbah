using System;
using System.Linq;
using Kasbah.Content.Models;
using Kasbah.Media.Models;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web
{
    public static class UrlExtensions
    {
        public static Uri ItemUrl(this Site site, Node node, bool absolute = false)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var relativePath = string.Join("/", node.Taxonomy.Aliases.Skip(site.ContentRoot.Count()));

            if (absolute)
            {
                var builder = new UriBuilder
                {
                    Scheme = site.UseSsl ? "https" : "http",
                    Host = site.DefaultHostname,
                    Port = site.DefaultPort ?? (site.UseSsl ? 443 : 80),
                    Path = relativePath
                };

                return builder.Uri;
            }

            return new Uri($"/{relativePath}", UriKind.Relative);
        }

        public static Uri ItemUrl(this KasbahWebContext kasbahWebContext, Node node, bool absolute = false)
            => ItemUrl(kasbahWebContext.Site, node, absolute);

        public static Uri ItemUrl(this KasbahWebContext kasbahWebContext, Item item, bool absolute = false)
            => ItemUrl(kasbahWebContext, item.Node, absolute);

        public static Uri ItemUrl(this IUrlHelper urlHelper, Item item, bool absolute = false)
        {
            var kasbahWebContext = urlHelper.ActionContext.HttpContext.GetKasbahWebContext();

            return kasbahWebContext.ItemUrl(item, absolute);
        }

        public static Uri MediaUrlAsync(this IUrlHelper urlHelper, MediaItem mediaItem, bool absolute = false)
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
                var builder = new UriBuilder
                {
                    Scheme = site.UseSsl ? "https" : "http",
                    Host = site.DefaultHostname,
                    Port = site.DefaultPort ?? (site.UseSsl ? 443 : 80),
                    Path = relativePath
                };

                return builder.Uri;
            }

            return new Uri($"/{relativePath}", UriKind.Relative);
        }
    }
}
