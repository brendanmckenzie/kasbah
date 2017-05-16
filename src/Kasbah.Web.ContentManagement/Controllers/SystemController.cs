using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Logging;
using Kasbah.Web.ContentManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
{
    [Authorize]
    [Route("system")]
    public class SystemController : Controller
    {
        readonly IEnumerable<ExternalModule> _externalModules;

        public SystemController(IEnumerable<ExternalModule> externalModules = null)
        {
            _externalModules = externalModules ?? Enumerable.Empty<ExternalModule>();
        }


        [Route("external-modules/list"), HttpGet, AllowAnonymous]
        public IEnumerable<ExternalModule> ListExternalModules()
            => _externalModules;
    }
}
