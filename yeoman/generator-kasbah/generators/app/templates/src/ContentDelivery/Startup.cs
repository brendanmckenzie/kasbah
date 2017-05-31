using Kasbah.Web.ContentDelivery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace <%= namespace %>.ContentDelivery
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKasbahPublic();
            services.Add<%= namespace %>Web();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();

            app.UseKasbahPublic();
            app.Use<%= namespace %>Web();
        }
    }
}
