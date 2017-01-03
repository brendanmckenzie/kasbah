using ExampleSite.Models;
using Kasbah.Content;
using Kasbah.DataAccess;
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
            typeRegistry.Register<ExampleModel>(config =>
            {
                config
                    .FieldHelpText(nameof(ExampleModel.Lorem), "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s")
                    .FieldDisplayName(nameof(ExampleModel.Lorem), "Lorem ipsum");
            });

            return app;
        }
    }
}
