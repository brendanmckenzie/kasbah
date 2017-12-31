using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Kasbah.Web.ContentManagement.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.ContentManagement
{
    public static class KasbahWebExtensions
    {
        static readonly IEnumerable<Client> AuthClients = new[]
        {
            new Client
            {
                ClientId = "web",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "kasbah" },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                }
            }
        };

        static readonly IEnumerable<ApiResource> ApiResources = new[]
        {
            new ApiResource("kasbah", "Kasbah API")
        };

        public static IServiceCollection AddKasbahAdmin(this IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "kasbah";
                    options.ApiSecret = "secret";
                });

            services.AddIdentityServer(options =>
            {
                options.IssuerUri = "http://localhost:5000";
            })
                .AddInMemoryClients(AuthClients)
                .AddInMemoryApiResources(ApiResources)
                .AddResourceOwnerValidator<UserResourceOwnerPasswordValidator>()
                .AddDeveloperSigningCredential();

            services.AddKasbah();
            services.AddKasbahWeb();

            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahWebExtensions).GetTypeInfo().Assembly);

            return services;
        }

        public static IApplicationBuilder UseKasbahAdmin(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseIdentityServer();

            app.UseMvc();

            InitialiseAsync(app.ApplicationServices).Wait();

            return app;
        }

        static async Task InitialiseAsync(IServiceProvider services)
        {
            await services.InitialiseKasbahAsync();
            await services.InitialiseKasbahWebAsync();
        }
    }
}
