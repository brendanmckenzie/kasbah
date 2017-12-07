using System;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.Delivery;
using Kasbah.Web.Management;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.EntryPoint
{
    public static class KasbahWebExtensions
    {
        static KasbahWebMode _mode;

        public static IServiceCollection AddKasbahWeb(this IServiceCollection services, KasbahWebMode mode)
        {
            _mode = mode;

            services.AddSingleton<SiteRegistry>();
            services.AddSingleton<ComponentRegistry>();

            var mvcBuilder = services.AddMvc();

            mvcBuilder.AddApplicationPart(typeof(KasbahWeb).GetTypeInfo().Assembly);

            if (mode.HasFlag(KasbahWebMode.Delivery))
            {
                Kasbah.Web.Delivery.Startup.ConfigureServices(services, mvcBuilder);
            }

            if (mode.HasFlag(KasbahWebMode.Management))
            {
                Kasbah.Web.Management.Startup.ConfigureServices(services, mvcBuilder);
            }

            return services;
        }

        public static IApplicationBuilder UseKasbahWeb(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                if (_mode.HasFlag(KasbahWebMode.Delivery))
                {
                    Kasbah.Web.Delivery.Startup.Configure(app, routes);
                }

                if (_mode.HasFlag(KasbahWebMode.Management))
                {
                    Kasbah.Web.Management.Startup.Configure(app, routes);
                }
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
