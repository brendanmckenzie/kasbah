using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.Middleware.Delivery;
using Kasbah.Web.Models;
using Kasbah.Web.Security.Management;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web
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

        public static IServiceCollection AddKasbahWeb(this IServiceCollection services)
        {
            services.AddKasbah();

            services.AddSingleton<SiteRegistry>();
            services.AddSingleton<ComponentRegistry>();

            return services;
        }

        public static IServiceCollection AddKasbahWebDelivery(this IServiceCollection services)
        {
            services.AddKasbahWeb();

            services.AddSingleton(new KasbahWebApplication());

            services.AddNodeServices();
            services.AddSpaPrerenderer();

            services.AddMvc()
                .AddApplicationPart(typeof(KasbahWeb).GetTypeInfo().Assembly);

            return services;
        }

        public static IServiceCollection AddKasbahWebManagement(this IServiceCollection services)
        {
            services.AddKasbahWeb();

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

            services.AddMvc()
                .AddApplicationPart(typeof(KasbahWeb).GetTypeInfo().Assembly);

            return services;
        }

        public static IApplicationBuilder UseKasbahWebDelivery(this IApplicationBuilder app)
        {
            app.UseMiddleware<KasbahWebContextInitialisationMiddleware>();
            app.UseMiddleware<SiteResolverMiddleware>();
            app.UseMiddleware<NodeResolverMiddleware>();
            app.UseMiddleware<KasbahRouterMiddleware>();

            app.UseMvc();

            InitialiseKasbahWeb(app.ApplicationServices);

            return app;
        }

        public static IApplicationBuilder UseKasbahWebManagement(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                app.UseAuthentication();
                app.UseIdentityServer();
            });

            InitialiseKasbahWeb(app.ApplicationServices);

            return app;
        }

        public static void InitialiseKasbahWeb(this IServiceProvider services)
        {
            var typeRegistry = services.GetService<TypeRegistry>();
            var siteRegistry = services.GetService<SiteRegistry>();
            var componentRegistry = services.GetService<ComponentRegistry>();

            typeRegistry.Register<Item>();
            typeRegistry.Register<Folder>();

            var registration = services.GetService<KasbahWebRegistration>();

            registration.RegisterTypes(typeRegistry);
            registration.RegisterSites(siteRegistry);
            registration.RegisterComponents(componentRegistry);

            services.InitialiseKasbahAsync().Wait();
        }
    }
}
