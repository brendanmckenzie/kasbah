using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Admin.Web
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbah(this IServiceCollection services)
        {
            services.AddTransient<SecurityService>();
            services.AddTransient<AnalyticsService>();
            services.AddTransient<ContentService>();

            return services;
        }
    }
}