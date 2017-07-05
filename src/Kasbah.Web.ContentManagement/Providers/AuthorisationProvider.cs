using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Kasbah.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.ContentManagement.Providers
{
    public class AuthorisationProvider : OpenIdConnectServerProvider
    {
        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (context.Request.IsPasswordGrantType())
            {
                var securityService = context.HttpContext.RequestServices.GetService<SecurityService>();
                try
                {
                    var user = await securityService.VerifyUserAsync(context.Request.Username, context.Request.Password);

                    var identity = new ClaimsIdentity(context.Options.AuthenticationScheme);
                    identity.AddClaim(ClaimTypes.Name, user.Username);
                    identity.AddClaim(ClaimTypes.NameIdentifier, user.Id.ToString());

                    var ticket = new AuthenticationTicket(
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties(),
                        context.Options.AuthenticationScheme);

                    // Call SetScopes with the list of scopes you want to grant
                    // (specify offline_access to issue a refresh token).
                    ticket.SetScopes(
                        OpenIdConnectConstants.Scopes.Profile,
                        OpenIdConnectConstants.Scopes.OfflineAccess);

                    context.Validate(ticket);
                }
                catch (UserNotFoundException)
                {
                    context.Reject(error: OpenIdConnectConstants.Errors.InvalidGrant, description: "Invalid user credentials.");
                }
                catch (InvalidLoginException)
                {
                    context.Reject(error: OpenIdConnectConstants.Errors.InvalidGrant, description: "Invalid user credentials.");
                }
            }
        }

        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only grant_type=password and refresh_token requests are accepted by this server.");

                return;
            }

            if (string.IsNullOrEmpty(context.ClientId))
            {
                context.Skip();
            }

            await Task.Delay(0);
        }
    }
}
