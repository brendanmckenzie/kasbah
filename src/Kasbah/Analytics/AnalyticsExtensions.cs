using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Analytics
{
    public static class AnalyticsExtensions
    {
        public static IServiceCollection AddAnalytics(this IServiceCollection services, Action<AnalyticsBus> config)
        {
            var bus = new AnalyticsBus();

            services.AddSingleton(bus);

            config(bus);

            return services;
        }
    }
}
