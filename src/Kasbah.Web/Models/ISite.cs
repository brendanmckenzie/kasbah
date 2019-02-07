using System.Collections.Generic;

namespace Kasbah.Web.Models
{
    public interface ISite
    {
        IEnumerable<string> Hostnames { get; set; }

        string DefaultHostname { get; set; }

        bool UseSsl { get; set; }
    }
}
