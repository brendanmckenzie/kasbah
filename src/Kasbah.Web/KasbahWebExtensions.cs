﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Media;
using Kasbah.Web.Controllers.Management;
using Kasbah.Web.Middleware.Delivery;
using Kasbah.Web.Models;
using Kasbah.Web.Security.Management;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
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
            services.AddMiniProfiler();

            services.AddKasbah();
            services.AddKasbahMedia();

            services.AddSingleton<SiteRegistry>();
            services.AddSingleton<ComponentRegistry>();

            return services;
        }

        public static IServiceCollection AddKasbahWebDelivery(this IServiceCollection services, IEnumerable<Assembly> applicationParts = null)
        {
            services.AddKasbahWeb();
            services.AddMemoryCache();

            services.AddSingleton(new KasbahWebApplication());

            services.AddNodeServices();
            services.AddSpaPrerenderer();

            var mvcBuilder = services.AddMvc();

            mvcBuilder.AddApplicationPart(typeof(KasbahWebExtensions).GetTypeInfo().Assembly);

            foreach (var ent in applicationParts ?? Enumerable.Empty<Assembly>())
            {
                mvcBuilder.AddApplicationPart(ent);
            }

            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                manager.FeatureProviders.Add(new DeliveryControllerFeatureProvider());
            });

            return services;
        }

        public static IServiceCollection AddKasbahWebManagement(this IServiceCollection services, IEnumerable<Assembly> applicationParts = null)
        {
            services.AddKasbahWeb();
            services.AddMemoryCache();

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

            var mvcBuilder = services.AddMvc();

            mvcBuilder.AddApplicationPart(typeof(KasbahWebExtensions).GetTypeInfo().Assembly);

            foreach (var ent in applicationParts ?? Enumerable.Empty<Assembly>())
            {
                mvcBuilder.AddApplicationPart(ent);
            }

            return services;
        }

        public static IApplicationBuilder UseKasbahWebDelivery(this IApplicationBuilder app, IEnumerable<Type> middleware = null)
        {
            app.UseMiniProfiler();

            app.UseMiddleware<KasbahProfilingMiddleware>();
            app.UseMiddleware<KasbahWebContextInitialisationMiddleware>();
            app.UseMiddleware<SiteResolverMiddleware>();
            app.UseMiddleware<NodeResolverMiddleware>();
            app.UseMiddleware<KasbahRouterMiddleware>();

            foreach (var ent in middleware ?? Enumerable.Empty<Type>())
            {
                app.UseMiddleware(ent);
            }

            app.UseMiddleware<KasbahContentMiddleware>();

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

        static void InitialiseKasbahWeb(this IServiceProvider services)
        {
            var typeRegistry = services.GetService<TypeRegistry>();
            var siteRegistry = services.GetService<SiteRegistry>();
            var componentRegistry = services.GetService<ComponentRegistry>();

            // Register default item types
            typeRegistry.Register<Item>();
            typeRegistry.Register<Folder>();

            var registration = services.GetService<KasbahWebRegistration>();

            if (registration == null)
            {
                throw new ArgumentNullException(nameof(KasbahWebRegistration), "An instance of KasbahWebRegistration must be registered");
            }

            // Register application-specific types. sites, components
            registration.RegisterTypes(typeRegistry);
            registration.RegisterSites(siteRegistry);
            registration.RegisterComponents(componentRegistry);

            // Perform underlying Kasbah initialisation
            services.InitialiseKasbahAsync().Wait();
        }

        public class DeliveryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
        {
            public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
            {
                // TODO: add a comment why this is necessary
                var remove = feature.Controllers
                    .Where(ent => ent.FullName.StartsWith(typeof(StaticContentController).Namespace))
                    .ToArray();

                foreach (var ent in remove)
                {
                    feature.Controllers.Remove(ent);
                }
            }
        }
    }
}
