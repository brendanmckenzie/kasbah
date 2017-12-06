using System;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.Delivery
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            services.AddSingleton(new KasbahWebApplication());
            services.AddTransient<KasbahRouter>();

            mvcBuilder.AddApplicationPart(typeof(Startup).GetTypeInfo().Assembly);

            return services;
        }

        public static IApplicationBuilder Configure(IApplicationBuilder app, IRouteBuilder routes)
        {
            app.UseMiddleware<Middleware.KasbahWebContextInitialisationMiddleware>();
            app.UseMiddleware<Middleware.SiteResolverMiddleware>();
            app.UseMiddleware<Middleware.NodeResolverMiddleware>();

            // TODO: See if it's possible to use middleware instead of a custom router
            var kasbahRouter = app.ApplicationServices.GetService<KasbahRouter>();
            routes.Routes.Add(kasbahRouter);
            routes.MapRoute(
                name: "default",
                template: "{*path}");

            return app;
        }
    }
}
