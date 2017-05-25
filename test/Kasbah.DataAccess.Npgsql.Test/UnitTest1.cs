using System;
using Xunit;
using Kasbah.DataAccess.Npgsql;
using Kasbah.Content;
using Kasbah.Content.Models;
using System.Linq;

namespace Kasbah.DataAccess.Npgsql.Test
{
    public class UnitTest1
    {   
        [Fact]
        public void TestMethod1()
        {
            var settings = new NpgsqlSettings { ConnectionString = "Server=localhost;Port=32769;Database=kasbah;User Id=kasbah;Password=kasbah" };
            var typeRegistry = new TypeRegistry();
            typeRegistry.Register<TestType>();
            var contentProvider = new ContentProvider(settings, typeRegistry);

            contentProvider.Query<TestType>().Where(ent => ent.TestProp == "hello").ToList();
        }

        class TestType : Item
        {
            public string TestProp { get; set; }
        }
    }
}