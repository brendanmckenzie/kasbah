namespace Kasbah.Analytics.Extensions
{
    public static class AnalyticsExtensions
    {
        public static IServiceCollection AddKasbahAnalytics(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UseKasbahAnalytics(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
