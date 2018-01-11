using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Kasbah.Security;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Security.Management
{
    public class UserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        readonly ILogger _log;
        readonly SecurityService _securityService;

        public UserResourceOwnerPasswordValidator(ILogger<UserResourceOwnerPasswordValidator> log, SecurityService securityService)
        {
            _log = log;
            _securityService = securityService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await _securityService.VerifyUserAsync(context.UserName, context.Password);

                if (user != null)
                {
                    context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password, Enumerable.Empty<Claim>());
                }
            }
            catch (UserNotFoundException)
            {
                _log.LogInformation($"Unknown user login attempt for user '{context.UserName}'");
            }
            catch (InvalidLoginException)
            {
                _log.LogInformation($"Invalid login attempt for user '{context.UserName}'");
            }
        }
    }
}
