using System.Reflection;
using Kasbah.Web.ContentManagement.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.ContentManagement
{
    public static class KasbahExtensions
    {
        public static IServiceCollection AddKasbahAdmin(this IServiceCollection services)
        {
            services.AddAuthentication();

            services.AddKasbah();

            services
                .AddMvc()
                .AddApplicationPart(typeof(KasbahExtensions).GetTypeInfo().Assembly);

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

            app.ApplicationServices.InitialiseKasbahAsync().Wait();

            return app;
        }
    }
}