using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Kasbah.Security;
using Kasbah.Security.Models;
using Kasbah.Web.Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Management.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("security")]
    public class SecurityController
    {
        readonly SecurityService _securityService;

        public SecurityController(SecurityService securityService)
        {
            _securityService = securityService;
        }

        [Route("users")]
        [HttpGet]
        public async Task<IEnumerable<User>> ListUsersAsync()
            => await _securityService.ListUsersAsync();

        [Route("users")]
        [HttpPost]
        public async Task<Guid> CreateUserAsync([FromBody] CreateUserRequest request)
            => await _securityService.CreateUserAsync(request.Username, request.Password, request.Name, request.Email);

        [Route("users")]
        [HttpPut]
        public async Task<User> PutUserAsync([FromBody] UpdateUserRequest request)
            => await _securityService.PutUserAsync(request.Id, request.Username, request.Password, request.Name, request.Email);
    }
}
