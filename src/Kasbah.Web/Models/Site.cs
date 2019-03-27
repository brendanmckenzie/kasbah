using System.Collections.Generic;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public class Site : Item, ISite
    {
        public string Alias { get; set; }

        public string Area { get; set; }

        public IEnumerable<string> Hostnames { get; set; }

        public IEnumerable<int> Ports { get; set; }

        public IEnumerable<string> ContentRoot { get; set; } // => Node.Taxonomy.Aliases;

        /// <summary>
        /// When resolving URLs from content this domain will be used.
        /// It does not need to be one of the domains listed in <c ref="Domains" />.
        /// </summary>
        /// <returns></returns>
        public string DefaultHostname { get; set; }

        public int? DefaultPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether URLs for this site should be https or http
        /// whether the http or https protocol is used.
        /// </summary>
        /// <returns></returns>
        public bool UseSsl { get; set; }
    }
}
