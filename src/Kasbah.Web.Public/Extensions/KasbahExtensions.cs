using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.Public
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbahPublic(this IServiceCollection services)
        {
            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahExtensions).GetTypeInfo().Assembly);

            services.AddSingleton<TypeRegistry>();
            services.AddSingleton<SiteRegistry>();

            services.AddTransient<AnalyticsService>();
            services.AddTransient<SecurityService>();
            services.AddTransient<ContentService>();

            services.AddTransient<KasbahRouter>();

            return services;
        }

        public static IApplicationBuilder UseKasbahPublic(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                var kasbahRouter = app.ApplicationServices.GetService<KasbahRouter>();
                routes.Routes.Add(kasbahRouter);
                routes.MapRoute(
                    name: "default",
                    template: "{*path}");
            });


            // Find a better way/place to do this...
            Task.WaitAll(
                app.ApplicationServices.GetService<ContentService>().InitialiseAsync(),
                app.ApplicationServices.GetService<SecurityService>().InitialiseAsync(),
                app.ApplicationServices.GetService<AnalyticsService>().InitialiseAsync());

            return app;
        }
    }
}