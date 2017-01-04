using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess.ElasticSearch;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Tests
{
    class TestType
    {
        public string Property { get; set; }
    }
    public class ElasticSearchDataAccessProviderTest
    {
        const string Index = "testing";
        ElasticSearchDataAccessProviderSettings _settings = new ElasticSearchDataAccessProviderSettings
        {
            Node = "http://localhost:32769"
        };

        ElasticSearchDataAccessProvider GetProvider()
            => new ElasticSearchDataAccessProvider(new LoggerFactory(), _settings);

        [Fact]
        public async Task PutEntry_WithValidDetails_CreatedSuccessfuly()
        {
            var id = Guid.NewGuid();
            var entry = new TestType { Property = "Hello world" };

            var provider = GetProvider();

            await provider.PutEntryAsync(Index, id, entry);
        }

        [Fact]
        public async Task PutEntryThenGetEntry_WithValidDetails_ReturnsEntry()
        {
            var id = Guid.NewGuid();
            var entry = new TestType { Property = "Hello world" };

            var provider = GetProvider();

            await provider.PutEntryAsync(Index, id, entry);

            var res = await provider.GetEntryAsync<TestType>(Index, id);

            Assert.NotNull(res);
            Assert.Equal(entry.Property, res.Source.Property);
            Assert.NotEqual(entry, res.Source);
        }

        [Fact]
        public async Task PutEntryThenQueryEntries_WithValidDetails_ReturnsEntry()
        {
            var id = Guid.NewGuid();
            var entry = new TestType { Property = "Hello world" };

            var provider = GetProvider();

            await provider.PutEntryAsync(Index, id, entry);
            await Task.Delay(1000);

            var res = await provider.QueryEntriesAsync<TestType>(Index, $"_id:{id}");
            Assert.NotNull(res);
            Assert.NotEmpty(res);
            Assert.Contains(id, res.Select(ent => ent.Id));
        }
    }
}
