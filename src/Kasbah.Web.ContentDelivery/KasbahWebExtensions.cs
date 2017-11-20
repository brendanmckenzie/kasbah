using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.ContentDelivery
{
    public static class KasbahWebExtensions
    {
        public static IServiceCollection AddKasbahPublic(this IServiceCollection services)
        {
            services.AddSingleton(new KasbahWebApplication());

            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahWebExtensions).GetTypeInfo().Assembly);

            services.AddKasbah();
            services.AddKasbahWeb();

            services.AddTransient<KasbahRouter>();

            return services;
        }

        public static IApplicationBuilder UseKasbahPublic(this IApplicationBuilder app, IEnumerable<Type> middleware = null)
        {
            app.UseMiddleware<Middleware.KasbahWebContextInitialisationMiddleware>();
            app.UseMiddleware<Middleware.SiteResolverMiddleware>();
            app.UseMiddleware<Middleware.NodeResolverMiddleware>();

            foreach (var ent in middleware ?? Enumerable.Empty<Type>())
            {
                app.UseMiddleware(ent);
            }

            app.UseMvc(routes =>
            {
                // TODO: See if it's possible to use middleware instead of a custom router
                var kasbahRouter = app.ApplicationServices.GetService<KasbahRouter>();
                routes.Routes.Add(kasbahRouter);
                routes.MapRoute(
                    name: "default",
                    template: "{*path}");
            });

            InitialiseAsync(app.ApplicationServices).Wait();

            return app;
        }

        static async Task InitialiseAsync(IServiceProvider services)
        {
            await services.InitialiseKasbahAsync();
            await services.InitialiseKasbahWebAsync();
        }
    }
}
