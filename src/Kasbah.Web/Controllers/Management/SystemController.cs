using System.Collections.Generic;
using System.Linq;
using IdentityServer4.AccessTokenValidation;
using Kasbah.Web.Models.Management;
using Kasbah.Web.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Controllers.Management
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("system")]
    public class SystemController : Controller
    {
        readonly SiteRegistry _siteRegistry;
        readonly IEnumerable<ExternalModule> _externalModules;

        public SystemController(SiteRegistry siteRegistry, IEnumerable<ExternalModule> externalModules = null)
        {
            _siteRegistry = siteRegistry;
            _externalModules = externalModules ?? Enumerable.Empty<ExternalModule>();
        }


        [Route("summary")]
        [HttpGet]
        public SystemSummary Summary()
        {
            return new SystemSummary
            {
                Version = "3.0.0",
                ExternalModules = _externalModules,
                Sites = _siteRegistry.ListSites()
            };
        }
    }
}
