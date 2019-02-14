using System.Collections.Generic;

namespace Kasbah.Web.Models
{
    public interface ISite
    {
        IEnumerable<string> Hostnames { get; set; }

        IEnumerable<int> Ports { get; set; }

        string DefaultHostname { get; set; }

        int? DefaultPort { get; set; }

        bool UseSsl { get; set; }
    }
}
