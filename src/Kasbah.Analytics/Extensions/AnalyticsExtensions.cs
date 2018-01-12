using Kasbah.Analytics;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah
{
    public static class AnalyticsExtensions
    {
        public static IServiceCollection AddKasbahAnalytics(this IServiceCollection services)
        {
            services.AddTransient<ManagementService>();
            services.AddTransient<ReportingService>();
            services.AddTransient<TrackingService>();
            services.AddTransient<SessionService>();

            return services;
        }
    }
}
