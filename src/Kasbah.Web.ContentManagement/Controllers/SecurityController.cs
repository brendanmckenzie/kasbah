using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kasbah.Security;
using Kasbah.Security.Models;

namespace Kasbah.Web.ContentManagement.Controllers
{
    [Authorize]
    [Route("security")]
    public class SecurityController
    {
        readonly SecurityService _securityService;
        public SecurityController(SecurityService securityService)
        {
            _securityService = securityService;
        }

        [Route("users"), HttpGet]
        public async Task<IEnumerable<User>> ListUsersAsync()
            => await _securityService.ListUsersAsync();
    }
}
