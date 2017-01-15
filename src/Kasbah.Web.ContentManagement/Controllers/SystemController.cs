using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
{
    [Authorize]
    [Route("system")]
    public class SystemController : Controller
    {
        readonly LoggingService _loggingService;

        public SystemController(LoggingService systemService)
        {
            _loggingService = systemService;
        }

        [Route("instances/list"), HttpGet]
        public async Task<IEnumerable<InstanceStatus>> List()
            => await _loggingService.ListActiveInstances();
    }
}