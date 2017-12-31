using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Kasbah.Web.ContentManagement.Models;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
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

        [Route("external-modules/list")]
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ExternalModule> ListExternalModules()
            => _externalModules;

        [Route("sites")]
        [HttpGet]
        public IEnumerable<Site> ListSites()
            => _siteRegistry.ListSites();
    }
}
