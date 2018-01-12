using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Provider.Npgsql
{
    public static class KasbahNpgsqlExtensions
    {
        public static IServiceCollection AddKasbahNpgsql(this IServiceCollection services, Action<Provider.Npgsql.NpgsqlSettings> configure)
        {
            var settings = new Provider.Npgsql.NpgsqlSettings();
            configure(settings);
            services.AddSingleton(settings);

            services.AddTransient<Kasbah.Content.IContentProvider, Kasbah.Provider.Npgsql.ContentProvider>();
            services.AddTransient<Kasbah.Security.IUserProvider, Kasbah.Provider.Npgsql.UserProvider>();
            services.AddTransient<Kasbah.Media.IMediaProvider, Kasbah.Provider.Npgsql.MediaProvider>();
            services.AddTransient<Kasbah.Analytics.IAnalyticsDataProvider, Kasbah.Provider.Npgsql.AnalyticsDataProvider>();
            services.AddTransient<Kasbah.Content.IKasbahQueryProviderFactory, Kasbah.Provider.Npgsql.KasbahQueryProviderFactory>();

            return services;
        }
    }
}
