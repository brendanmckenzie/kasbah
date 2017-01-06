using System.IO;
using ExampleSite.Models;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.DataAccess;
using Kasbah.Media;
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

            services.AddSingleton(new Kasbah.Media.LocalStorageMediaProviderSettings
            {
                ContentRoot = Path.Combine(Directory.GetCurrentDirectory(), "media")
            });
            services.AddTransient<IMediaStorageProvider, Kasbah.Media.LocalStorageMediaProvider>();

            return services;
        }

        public static IApplicationBuilder UseExampleSite(this IApplicationBuilder app)
        {
            var typeRegistry = app.ApplicationServices.GetService<TypeRegistry>();
            typeRegistry.Register<Folder>();
            typeRegistry.Register<WebRoot>();
            typeRegistry.Register<HomePage>(config =>
            {
                config
                    .FieldEditor(nameof(HomePage.LongText), "longText")
                    .FieldEditor(nameof(HomePage.ShortText), "text")
                    .FieldEditor(nameof(HomePage.Date), "date")
                    .SetOption("view", "SomeView");
            });
            typeRegistry.Register<ExampleModel>(config =>
            {
                config
                    .FieldHelpText(nameof(ExampleModel.Lorem), "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s")
                    .FieldDisplayName(nameof(ExampleModel.Lorem), "Lorem ipsum");
            });

            var siteRegistry = app.ApplicationServices.GetService<SiteRegistry>();
            siteRegistry.RegisterSite(config =>
            {
                config.Alias = "takeoffgo";
                config.Domains = new[] { "localhost:5001" };
                config.ContentRoot = new[] { "sites", "takeoffgo", "home" };
            });

            return app;
        }
    }
}
