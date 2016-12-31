using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.DataAccess;
using Kasbah.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kasbah.Admin.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddDefaultTokenProviders();

            services.AddAuthentication();

            services.AddMvc();

            // Kasbah services
            // TODO: Create .AddKasbah() extension method
            services.AddTransient<SecurityService>();
            services.AddTransient<AnalyticsService>();
            services.AddTransient<ContentService>();

            // TODO: remove dependency on ES provider
            services.AddSingleton(new Kasbah.DataAccess.ElasticSearch.ElasticSearchDataAccessProviderSettings
            {
                Node = "http://localhost:32769"
            });
            services.AddTransient<IDataAccessProvider, Kasbah.DataAccess.ElasticSearch.ElasticSearchDataAccessProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseOAuthValidation();

            app.UseStaticFiles();

            // app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
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
                                description: "Only grant_type=password and refresh_token " +
                                             "requests are accepted by this server.");

                            return Task.FromResult(0);
                        }

                        // Note: you can skip the request validation when the client_id
                        // parameter is missing to support unauthenticated token requests.
                        // if (string.IsNullOrEmpty(context.ClientId)) {
                        //     context.Skip();
                        //
                        //     return Task.FromResult(0);
                        // }

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
                        // Only handle grant_type=password token requests and let the
                        // OpenID Connect server middleware handle the other grant types.
                        if (context.Request.IsPasswordGrantType())
                        {
                            var securityService = context.HttpContext.RequestServices.GetService<SecurityService>();
                            // Implement context.Request.Username/context.Request.Password validation here.
                            // Note: you can call context Reject() to indicate that authentication failed.
                            // Using password derivation and time-constant comparer is STRONGLY recommended.
                            try
                            {
                                var user = await securityService.VerifyUserAsync(context.Request.Username, context.Request.Password);

                                var identity = new ClaimsIdentity(context.Options.AuthenticationScheme);
                                identity.AddClaim(ClaimTypes.NameIdentifier, user.Username);

                                // By default, claims are not serialized in the access/identity tokens.
                                // Use the overload taking a "destinations" parameter to make sure
                                // your claims are correctly inserted in the appropriate tokens.
                                // identity.AddClaim("urn:customclaim", "value",
                                //     OpenIdConnectConstants.Destinations.AccessToken,
                                //     OpenIdConnectConstants.Destinations.IdentityToken);

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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Find a better way/place to do this...
            Task.WaitAll(
                app.ApplicationServices.GetService<ContentService>().InitialiseAsync(),
                app.ApplicationServices.GetService<SecurityService>().InitialiseAsync());
        }
    }
}
