using ExampleSite.Models;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.DataAccess;
using Kasbah.Web;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleSite
{
    public static class ExampleSiteExtensions
    {
        public static IServiceCollection AddExampleSite(this IServiceCollection services)
        {
            services.AddSingleton(new Kasbah.DataAccess.ElasticSearch.ElasticSearchDataAccessProviderSettings
            {
                Node = "http://localhost:32769"
            });
            services.AddTransient<IDataAccessProvider, Kasbah.DataAccess.ElasticSearch.ElasticSearchDataAccessProvider>();

            return services;
        }

        public static IApplicationBuilder UseExampleSite(this IApplicationBuilder app)
        {
            var typeRegistry = app.ApplicationServices.GetService<TypeRegistry>();
            typeRegistry.Register<Folder>();
            typeRegistry.Register<WebRoot>();
            typeRegistry.Register<HomePage>();
            typeRegistry.Register<ExampleModel>(config =>
            {
                config
                    .FieldHelpText(nameof(ExampleModel.Lorem), "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s")
                    .FieldDisplayName(nameof(ExampleModel.Lorem), "Lorem ipsum");
            });

            var siteRegistry = app.ApplicationServices.GetService<SiteRegistry>();
            siteRegistry.RegisterSite(config =>
            {
                config.Alias = "example";
                config.Domains = new[] { "localhost:5000" };
                config.ContentRoot = new[] { "sites", "example", "home" };
            });

            return app;
        }
    }
}
