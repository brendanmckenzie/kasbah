using System.Collections.Generic;
using Kasbah.Web.Models;
using Kasbah.Web.Models.Management;

namespace Kasbah.Web.ViewModels.Management
{
    public class SystemSummary
    {
        public string Version { get; set; }

        public IEnumerable<ExternalModule> ExternalModules { get; set; }

        public IEnumerable<Site> Sites { get; internal set; }
    }
}
