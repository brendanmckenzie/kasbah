using System;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbah(this IServiceCollection services)
        {
            services.AddSingleton<TypeMapper>();
            services.AddSingleton<PropertyMapper>();
            services.AddSingleton<TypeRegistry>();

            services.AddTransient<Content.Events.EventBus>();

            services.AddTransient<SecurityService>();
            services.AddTransient<ContentService>();

            return services;
        }

        public static async Task InitialiseKasbahAsync(this IServiceProvider services)
        {
            var typeRegistry = services.GetService<TypeRegistry>();
            typeRegistry.Register<Folder>();

            await Task.WhenAll(
                services.GetService<ContentService>().InitialiseAsync(),
                services.GetService<SecurityService>().InitialiseAsync());
        }
    }
}
