using System.Collections.Generic;

namespace Kasbah.Web.Models
{
    public class Site
    {
        public string Alias { get; set; }

        public IEnumerable<string> Domains { get; set; }

        public IEnumerable<string> ContentRoot { get; set; }

        /// <summary>
        /// When resolving URLs from content this domain will be used.
        /// It does not need to be one of the domains listed in <c ref="Domains" />.
        /// </summary>
        /// <returns></returns>
        public string DefaultDomain { get; set; }

        /// <summary>
        /// When resolving URLs from content this flag will determine
        /// whether the http or https protocol is used.
        /// </summary>
        /// <returns></returns>
        public bool UseSsl { get; set; }
    }
}
