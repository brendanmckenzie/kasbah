using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Media
{
    public static class KasbahMediaExtensions
    {
        public static IServiceCollection AddKasbahMedia(this IServiceCollection services)
        {
            services.AddTransient<MediaService>();
            services.AddSingleton<MediaTypeHandler>();

            return services;
        }
    }
}
