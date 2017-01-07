using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess.ElasticSearch;
using Kasbah.Content;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tests
{
    class TestType
    {
        public string Property { get; set; }
    }

    class TestTypeStringProp
    {
        public string StringProp { get; set; }
    }

    class TestTypeIntProp
    {
        public int IntProp { get; set; }
    }

    class TestTypeNestedProp
    {
        public NestedType NestedTypeProp { get; set; }
    }

    class NestedType
    {
        public string StringProp { get; set; }
    }

    public class TypeMapperTests
    {
        const string Index = "testing";
        ElasticSearchDataAccessProviderSettings _settings = new ElasticSearchDataAccessProviderSettings
        {
            Node = "http://localhost:32769"
        };

        ElasticSearchDataAccessProvider GetProvider()
            => new ElasticSearchDataAccessProvider(new LoggerFactory(), _settings);

        TypeRegistry _typeRegistry = new TypeRegistry();
        readonly ContentService _contentService;

        public TypeMapperTests()
        {
            _typeRegistry.Register<TestType>();
            _typeRegistry.Register<TestTypeStringProp>();
            _typeRegistry.Register<TestTypeIntProp>();
            _typeRegistry.Register<TestTypeNestedProp>();

            _contentService = new ContentService(new LoggerFactory(), GetProvider(), _typeRegistry);
        }

        [Fact]
        public async Task MapType_WithStringProp_ReturnsCorrectObject()
        {
            var typeMapper = new TypeMapper(_contentService, _typeRegistry);

            var dict = new Dictionary<string, object>
            {
                { "StringProp", "value" }
            };

            var obj = await typeMapper.MapTypeAsync(dict, typeof(TestTypeStringProp).AssemblyQualifiedName) as TestTypeStringProp;

            Assert.NotNull(obj);
            Assert.Equal(obj.StringProp, dict["StringProp"]);
        }

        [Fact]
        public async Task MapType_WithNestedObjProp_ReturnsCorrectObject()
        {
            var typeMapper = new TypeMapper(_contentService, _typeRegistry);

            var json = @"{'NestedTypeProp': { 'StringProp': 'value' } }";
            var dict = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

            var obj = await typeMapper.MapTypeAsync(dict, typeof(TestTypeNestedProp).AssemblyQualifiedName) as TestTypeNestedProp;

            Assert.NotNull(obj);
            Assert.NotNull(obj.NestedTypeProp);
            Assert.Equal(obj.NestedTypeProp.StringProp, "value");
        }

        [Fact]
        public async Task MapType_WithMismatchTypes_ReturnsCorrectObject()
        {
            var typeMapper = new TypeMapper(_contentService, _typeRegistry);

            var dict = new Dictionary<string, object>
            {
                { "IntProp", (decimal)1 }
            };

            var obj = await typeMapper.MapTypeAsync(dict, typeof(TestTypeIntProp).AssemblyQualifiedName) as TestTypeIntProp;

            Assert.NotNull(obj);
            Assert.Equal(obj.IntProp, 1);
        }
    }
}
