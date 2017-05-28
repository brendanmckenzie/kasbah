using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.DataAccess
{
    public static class KasbahNpgsqlExtensions
    {
        public static IServiceCollection AddKasbahNpgsql(this IServiceCollection services, Action<Npgsql.NpgsqlSettings> configure)
        {
            var settings = new Npgsql.NpgsqlSettings();
            configure(settings);
            services.AddSingleton(settings);

            services.AddTransient<Kasbah.Content.IContentProvider, Kasbah.DataAccess.Npgsql.ContentProvider>();
            services.AddTransient<Kasbah.Security.IUserProvider, Kasbah.DataAccess.Npgsql.UserProvider>();
            services.AddTransient<Kasbah.Media.IMediaProvider, Kasbah.DataAccess.Npgsql.MediaProvider>();
            services.AddTransient<Kasbah.Content.IKasbahQueryProviderFactory, Kasbah.DataAccess.Npgsql.KasbahQueryProviderFactory>();

            return services;
        }
    }
}
