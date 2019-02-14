using System;
using System.Collections.Generic;
using System.Linq;
using Kasbah.Content.Models;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Kasbah.Web
{
    public class SiteRegistry
    {
        readonly ICollection<Site> _sites;

        public SiteRegistry()
        {
            _sites = new List<Site>();
        }

        public void RegisterSite(Site site)
            => _sites.Add(site);

        public IEnumerable<Site> ListSites()
            => _sites.AsEnumerable();

        public Site GetSite(string alias)
            => _sites.SingleOrDefault(ent => ent.Alias == alias);

        public Site GetSiteByDomain(HostString domain)
            => _sites.FirstOrDefault(ent => (ent.Hostnames ?? Enumerable.Empty<string>()).Any(hst => hst.Equals(domain.Host))
                || (domain.Port.HasValue && (ent.Ports ?? Enumerable.Empty<int>()).Contains(domain.Port.Value)));

        public Site GetSiteByNode(Node node)
        {
            var taxonomy = node.Taxonomy.Aliases;

            return _sites.FirstOrDefault(ent => ent.ContentRoot.SequenceEqual(taxonomy.Take(ent.ContentRoot.Count())));
        }
    }
}
