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
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbahWeb(this IServiceCollection services)
        {
            services.AddSingleton<SiteRegistry>();

            return services;
        }

        public static async Task InitialiseKasbahWebAsync(this IServiceProvider services)
        {
            var typeRegistry = services.GetService<TypeRegistry>();
            var siteRegistry = services.GetService<SiteRegistry>();

            var registration = services.GetService<IKasbahWebRegistration>();

            registration.RegisterTypes(typeRegistry);
            registration.RegisterSites(siteRegistry);

            await Task.Yield();
        }
    }
}
