using System;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Media;
using Kasbah.Security;
using Kasbah.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web
{
    public static class KasbahWebExtensions
    {
        public static IServiceCollection AddKasbahWeb(this IServiceCollection services)
        {
            services.AddSingleton<SiteRegistry>();
            services.AddSingleton<ComponentRegistry>();

            return services;
        }

        public static async Task InitialiseKasbahWebAsync(this IServiceProvider services)
        {
            var typeRegistry = services.GetService<TypeRegistry>();
            var siteRegistry = services.GetService<SiteRegistry>();
            var componentRegistry = services.GetService<ComponentRegistry>();

            typeRegistry.Register<Folder>();

            var registration = services.GetService<KasbahWebRegistration>();

            registration.RegisterTypes(typeRegistry);
            registration.RegisterSites(siteRegistry);
            registration.RegisterComponents(componentRegistry);

            await Task.Yield();
        }
    }
}
