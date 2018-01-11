using Kasbah.Web.Analytics.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah
{
    public static class WebAnalyticsExtensions
    {
        public static IServiceCollection AddKasbahWebAnalytics(this IServiceCollection services)
        {
            services.AddKasbahAnalytics();

            return services;
        }

        public static IApplicationBuilder UseKasbahWebDelivery(this IApplicationBuilder app)
        {
            app.UseMiddleware<AnalyticsMiddleware>();

            return app;
        }
    }
}
