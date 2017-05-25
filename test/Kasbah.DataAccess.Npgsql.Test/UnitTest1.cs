using System;
using Xunit;
using Kasbah.DataAccess.Npgsql;
using Kasbah.Content;
using Kasbah.Content.Models;
using System.Linq;
using Kasbah.Content.Events;

namespace Kasbah.DataAccess.Npgsql.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            var settings = new NpgsqlSettings { ConnectionString = "Server=localhost;Port=5433;Database=kasbah;User Id=kasbah;Password=kasbah" };
            var typeRegistry = new TypeRegistry();
            typeRegistry.Register<TestType>();
            var contentProvider = new ContentProvider(settings, typeRegistry);
            var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            var eventBus = new EventBus(Enumerable.Empty<IOnContentPublished>());

            var typeMapper = new TypeMapper(loggerFactory, contentProvider, null, typeRegistry);
            var queryProviderFactory = new KasbahQueryProviderFactory(settings, typeRegistry, typeMapper);
            var contentService = new ContentService(loggerFactory, contentProvider, typeRegistry, eventBus, queryProviderFactory);


            contentService.Query<TestType>()
                // .Where(ent => ent.TestProp == "hello" && ent.PestTrop != "olleh")
                .Where(ent => ent.PestTrop.Contains("bye"))
                .Skip(0)
                .Take(5)
                .ToList();
        }

        class TestType : Item
        {
            public string TestProp { get; set; }
            public string PestTrop { get; set; }
        }
    }
}
