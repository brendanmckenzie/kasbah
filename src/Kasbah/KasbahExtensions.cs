using System;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Logging;
using Kasbah.Media;
using Kasbah.Security;
using Kasbah.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbah(this IServiceCollection services)
        {
            services.AddSingleton<TypeRegistry>();
            services.AddSingleton<SiteRegistry>();

            services.AddTransient<AnalyticsService>();
            services.AddTransient<SecurityService>();
            services.AddTransient<ContentService>();
            services.AddTransient<MediaService>();
            services.AddTransient<LoggingService>();

            return services;
        }

        public static async Task InitialiseKasbahAsync(this IServiceProvider services)
        {
            await services.GetService<AnalyticsService>().InitialiseAsync();
            await services.GetService<ContentService>().InitialiseAsync();
            await services.GetService<MediaService>().InitialiseAsync();
            await services.GetService<SecurityService>().InitialiseAsync();

            var typeRegistry = services.GetService<TypeRegistry>();
            typeRegistry.Register<Folder>();

            var siteRegistry = services.GetService<SiteRegistry>();

            var registration = services.GetService<IKasbahWebRegistration>();

            registration.RegisterTypes(typeRegistry);
            registration.RegisterSites(siteRegistry);
        }
    }
}