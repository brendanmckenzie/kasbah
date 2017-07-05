using System;
using Kasbah.Web.Models;

namespace Kasbah.Web
{
    public static class SiteRegistryExtensions
    {
        public static void RegisterSite(this SiteRegistry siteRegistry, Action<Site> config)
        {
            var site = new Site();

            config?.Invoke(site);

            siteRegistry.RegisterSite(site);
        }
    }
}
