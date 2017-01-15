using System;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Logging;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.ContentDelivery
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbahPublic(this IServiceCollection services)
        {
            services.AddSingleton(new KasbahWebApplication());

            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahExtensions).GetTypeInfo().Assembly);

            services.AddKasbah();

            services.AddTransient<KasbahRouter>();

            Jobs.Configure.RegisterJobs(services);

            return services;
        }

        public static IApplicationBuilder UseKasbahPublic(this IApplicationBuilder app)
        {
            app.UseMiddleware<Middleware.AnalyticsMiddleware>();
            app.UseMvc(routes =>
            {
                var kasbahRouter = app.ApplicationServices.GetService<KasbahRouter>();
                routes.Routes.Add(kasbahRouter);
                routes.MapRoute(
                    name: "default",
                    template: "{*path}");
            });

            InitialiseAsync(app.ApplicationServices).Wait();

            return app;
        }

        // Find a better way/place to do this...
        static async Task InitialiseAsync(IServiceProvider services)
        {
            var application = services.GetService<KasbahWebApplication>();

            await services.GetService<ContentService>().InitialiseAsync();
            await services.GetService<AnalyticsService>().InitialiseAsync();

            var loggingService = services.GetService<LoggingService>();

            await loggingService.InitialiseAsync();
            await loggingService.RegisterInstanceAsync(application.Id, application.Started);

            await Jobs.Configure.ConfigureJobsAsync(services);

            var typeRegistry = services.GetService<TypeRegistry>();
            var siteRegistry = services.GetService<SiteRegistry>();
            var registration = services.GetService<IKasbahWebRegistration>();

            registration.RegisterTypes(typeRegistry);
            registration.RegisterSites(siteRegistry);
        }
    }
}