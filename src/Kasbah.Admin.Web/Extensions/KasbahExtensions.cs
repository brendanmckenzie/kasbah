using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Admin.Web
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbahAdmin(this IServiceCollection services)
        {
            services.AddAuthentication();

            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahExtensions).GetTypeInfo().Assembly);

            services.AddSingleton<TypeRegistry>();

            services.AddTransient<SecurityService>();
            services.AddTransient<AnalyticsService>();
            services.AddTransient<ContentService>();

            return services;
        }

        public static IApplicationBuilder UseKasbahAdmin(this IApplicationBuilder app)
        {
            app.UseOAuthValidation();

            app.UseOpenIdConnectServer(options =>
            {
                options.SigningCredentials.AddEphemeralKey();
                options.AllowInsecureHttp = true;
                options.TokenEndpointPath = "/connect/token";
                options.Provider = new OpenIdConnectServerProvider
                {
                    // Implement OnValidateTokenRequest to support flows using the token endpoint.
                    OnValidateTokenRequest = context =>
                    {
                        // Reject token requests that don't use grant_type=password or grant_type=refresh_token.
                        if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
                        {
                            context.Reject(
                                error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                                description: "Only grant_type=password and refresh_token requests are accepted by this server.");

                            return Task.FromResult(0);
                        }

                        // Note: you can skip the request validation when the client_id
                        // parameter is missing to support unauthenticated token requests.
                        if (string.IsNullOrEmpty(context.ClientId))
                        {
                            context.Skip();

                            return Task.FromResult(0);
                        }

                        // Note: to mitigate brute force attacks, you SHOULD strongly consider applying
                        // a key derivation function like PBKDF2 to slow down the secret validation process.
                        // You SHOULD also consider using a time-constant comparer to prevent timing attacks.
                        if (string.Equals(context.ClientId, "client_id", StringComparison.Ordinal) &&
                                        string.Equals(context.ClientSecret, "client_secret", StringComparison.Ordinal))
                        {
                            context.Validate();
                        }

                        // Note: if Validate() is not explicitly called,
                        // the request is automatically rejected.
                        return Task.FromResult(0);
                    },

                    // Implement OnHandleTokenRequest to support token requests.
                    OnHandleTokenRequest = async context =>
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
                                context.Reject(
        error: OpenIdConnectConstants.Errors.InvalidGrant,
        description: "Invalid user credentials.");
                            }
                            catch (InvalidLoginException)
                            {
                                context.Reject(
        error: OpenIdConnectConstants.Errors.InvalidGrant,
        description: "Invalid user credentials.");
                            }
                        }
                    }
                };
            });

            app.UseMvc();


            // Find a better way/place to do this...
            Task.WaitAll(
                app.ApplicationServices.GetService<ContentService>().InitialiseAsync(),
                app.ApplicationServices.GetService<SecurityService>().InitialiseAsync());

            return app;
        }
    }
}