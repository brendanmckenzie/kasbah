using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Analytics.Extensions
{
    public static class AnalyticsExtensions
    {
        public static IServiceCollection AddKasbahAnalytics(this IServiceCollection services)
        {
            services.AddTransient<ManagementService>();
            services.AddTransient<ReportingService>();
            services.AddTransient<TrackingService>();

            return services;
        }
    }
}
