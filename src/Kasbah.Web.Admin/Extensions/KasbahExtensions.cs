using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Web.Admin.Providers;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.Admin
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbahAdmin(this IServiceCollection services)
        {
            services.AddAuthentication();

            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahExtensions).GetTypeInfo().Assembly);

            services.AddSingleton<TypeRegistry>();
            services.AddSingleton<SiteRegistry>();

            services.AddTransient<AnalyticsService>();
            services.AddTransient<SecurityService>();
            services.AddTransient<ContentService>();

            return services;
        }

        public static IApplicationBuilder UseKasbahAdmin(this IApplicationBuilder app)
        {
            app.UseOAuthValidation();

            app.UseOpenIdConnectServer(options =>
            {
                options.SigningCredentials.AddEphemeralKey();
                options.AllowInsecureHttp = true;
                options.TokenEndpointPath = "/connect/token";
                options.Provider = new AuthorisationProvider();
            });

            app.UseMvc();


            // Find a better way/place to do this...
            Task.WaitAll(
                app.ApplicationServices.GetService<ContentService>().InitialiseAsync(),
                app.ApplicationServices.GetService<SecurityService>().InitialiseAsync(),
                app.ApplicationServices.GetService<AnalyticsService>().InitialiseAsync());

            return app;
        }
    }
}