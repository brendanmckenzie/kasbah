using System;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Provider.Npgsql
{
    public static class KasbahNpgsqlExtensions
    {
        public static IServiceCollection AddKasbahNpgsql(this IServiceCollection services, Action<Provider.Npgsql.NpgsqlSettings> configure)
        {
            SqlMapper.AddTypeHandler(new DictionaryTypeHandler());

            var settings = new Provider.Npgsql.NpgsqlSettings();
            configure(settings);
            services.AddSingleton(settings);

            services.AddTransient<Kasbah.Content.IContentProvider, Kasbah.Provider.Npgsql.ContentProvider>();
            services.AddTransient<Kasbah.Security.IUserProvider, Kasbah.Provider.Npgsql.UserProvider>();
            services.AddTransient<Kasbah.Media.IMediaProvider, Kasbah.Provider.Npgsql.MediaProvider>();
            services.AddTransient<Kasbah.Analytics.IAnalyticsDataProvider, Kasbah.Provider.Npgsql.AnalyticsDataProvider>();
            services.AddTransient<Kasbah.Content.IKasbahQueryProviderFactory, Kasbah.Provider.Npgsql.KasbahQueryProviderFactory>();

            services.AddTransient<DatabaseUtility>();

            return services;
        }

        public static void UseKasbahNpgsql(this IServiceProvider services)
        {
            var settings = services.GetRequiredService<Provider.Npgsql.NpgsqlSettings>();

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "You must call AddKasbahNpgsql()");
            }

            if (string.IsNullOrEmpty(settings.ConnectionString))
            {
                throw new ArgumentNullException(nameof(settings.ConnectionString), "No connection string specified");
            }

            var utility = services.GetRequiredService<DatabaseUtility>();

            utility.InitialiseSchema(settings);
        }
    }
}
