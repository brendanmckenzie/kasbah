using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Web.Admin.Providers;
using Kasbah.Analytics;
using Kasbah.Content;
using Kasbah.Media;
using Kasbah.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            services.AddTransient<MediaService>();

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
            InitialiseAsync(app.ApplicationServices).Wait();

            return app;
        }

        static async Task InitialiseAsync(IServiceProvider services)
        {
            await services.GetService<AnalyticsService>().InitialiseAsync();
            await services.GetService<ContentService>().InitialiseAsync();
            await services.GetService<MediaService>().InitialiseAsync();
            await services.GetService<SecurityService>().InitialiseAsync();
        }
    }
}