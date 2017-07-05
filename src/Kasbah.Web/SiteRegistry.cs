using System;
using System.Collections.Generic;
using System.Linq;
using Kasbah.Content.Models;
using Kasbah.Web.Models;

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

        public Site GetSiteByDomain(string domain)
            => _sites.FirstOrDefault(ent => ent.Domains.Contains(domain));

        public Site GetSiteByNode(Node node)
        {
            var taxonomy = node.Taxonomy.Aliases;

            return _sites.FirstOrDefault(ent => ent.ContentRoot.SequenceEqual(taxonomy.Take(ent.ContentRoot.Count())));
        }
    }
}
