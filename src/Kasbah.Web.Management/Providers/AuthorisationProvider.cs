using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Kasbah.Security;

namespace Kasbah.Web.Management.Providers
{
    public class AuthorisationProvider : OpenIdConnectServerProvider
    {
        readonly SecurityService _securityService;

        public AuthorisationProvider(SecurityService securityService)
        {
            _securityService = securityService;
        }

        // public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        // {
        //     if (context.Request.IsPasswordGrantType())
        //     {
        //         try
        //         {
        //             var user = await _securityService.VerifyUserAsync(context.Request.Username, context.Request.Password);

        //             var identity = new ClaimsIdentity(
        //                 "Bearer",
        //                 OpenIdConnectConstants.Claims.Name,
        //                 OpenIdConnectConstants.Claims.Role);

        //             identity.AddClaim(
        //                 OpenIdConnectConstants.Claims.Subject,
        //                 user.Id.ToString(),
        //                 OpenIdConnectConstants.Destinations.AccessToken,
        //                 OpenIdConnectConstants.Destinations.IdentityToken);

        //             identity.AddClaim(
        //                 OpenIdConnectConstants.Claims.Name,
        //                 user.Username,
        //                 OpenIdConnectConstants.Destinations.AccessToken,
        //                 OpenIdConnectConstants.Destinations.IdentityToken);

        //             context.Validate(new ClaimsPrincipal(identity));
        //         }
        //         catch (UserNotFoundException)
        //         {
        //             context.Reject("Invalid user credentials.");
        //         }
        //         catch (InvalidLoginException)
        //         {
        //             context.Reject("Invalid user credentials.");
        //         }
        //     }
        // }

        // public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        // {
        //     if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
        //     {
        //         context.Reject(
        //             error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
        //             description: "Only grant_type=password and refresh_token requests are accepted by this server.");

        //         return;
        //     }

        //     if (string.IsNullOrEmpty(context.ClientId))
        //     {
        //         context.Skip();
        //     }

        //     await Task.Yield();
        // }
    }
}
