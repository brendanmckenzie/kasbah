using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kasbah.Web.Management
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            services.AddAuthentication().AddOpenIdConnectServer(options =>
            {
                options.SigningCredentials.AddEphemeralKey();
                options.AllowInsecureHttp = true;
                options.TokenEndpointPath = "/connect/token";
                options.Provider = new AuthorisationProvider();
            });

            mvcBuilder.AddApplicationPart(typeof(Startup).GetTypeInfo().Assembly);

            return services;
        }

        public static IApplicationBuilder Configure(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            InitialiseAsync(app.ApplicationServices).Wait();

            return app;
        }
    }
}
