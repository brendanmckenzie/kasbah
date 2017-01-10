using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kasbah.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kasbah.DataAccess.ElasticSearch
{
    public class ElasticSearchDataAccessProvider : IDataAccessProvider
    {
        readonly ElasticSearchDataAccessProviderSettings _settings;
        readonly HttpClient _webClient;
        readonly ILogger _log;

        public ElasticSearchDataAccessProvider(ILoggerFactory loggerFactory, ElasticSearchDataAccessProviderSettings settings)
        {
            _log = loggerFactory.CreateLogger<ElasticSearchDataAccessProvider>();

            _settings = settings;
            _webClient = new HttpClient
            {
                BaseAddress = new Uri(settings.Node)
            };
        }

        public async Task PutEntryAsync<T>(string index, Guid id, T data, Guid? parent = null)
        {
            var json = JsonConvert.SerializeObject(data);

            await _webClient.PutAsync(ItemUri<T>(index, id, parent, waitForRefresh: true), new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public async Task PutEntryAsync<T>(string index, Guid id, IDictionary<string, object> data, Guid? parent = null)
            => await PutEntryAsync(index, id, typeof(T), data, parent);

        public async Task PutEntryAsync(string index, Guid id, Type type, IDictionary<string, object> data, Guid? parent = null)
        {
            if (data?.ContainsKey("_version") == true)
            {
                data.Remove("_version");
            }
            var json = JsonConvert.SerializeObject(data);

            await _webClient.PutAsync(ItemUri(type, index, id, parent, waitForRefresh: true), new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public async Task DeleteEntryAsync<T>(string index, Guid id)
        {
            await _webClient.DeleteAsync(ItemUri<T>(index, id));
        }

        public async Task<IEnumerable<EntryWrapper<T>>> QueryEntriesAsync<T>(string index, object query = null)
        {
            var queryObj = ParseQuery(query);
            var queryStr = queryObj == null ? null : JsonConvert.SerializeObject(queryObj);

            var uri = new Uri($"{IndexName(index)}/{typeof(T).FullName}/_search", UriKind.Relative);

            _log.LogDebug($"{nameof(QueryEntriesAsync)}: {uri} - {queryStr}");

            var req = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Content = queryStr == null ? null : new StringContent(queryStr, Encoding.UTF8, "application/json")
            };

            var res = await _webClient.SendAsync(req);
            var resStr = await res.Content.ReadAsStringAsync();

            _log.LogDebug($"Result: {resStr}");

            var searchResult = JsonConvert.DeserializeObject<SearchResult<T>>(resStr);

            return searchResult?.Hits?.Hits?.Select(ent => new EntryWrapper<T>
            {
                Id = Guid.Parse(ent.Id),
                Source = ent.Source,
                Version = ent.Version
            }) ?? Enumerable.Empty<EntryWrapper<T>>();
        }

        public async Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, int? version = null)
        {
            var res = await _webClient.GetStringAsync(ItemUri<T>(index, id, version: version));
            var ent = JsonConvert.DeserializeObject<SearchResultHitsHit<T>>(res);

            return new EntryWrapper<T>
            {
                Id = Guid.Parse(ent.Id),
                Source = ent.Source,
                Version = ent.Version
            };
        }

        public async Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, Type type, int? version = null)
        {
            var res = await _webClient.GetStringAsync(ItemUri(type, index, id, version: version));
            var ent = JsonConvert.DeserializeObject<SearchResultHitsHit<T>>(res);

            return new EntryWrapper<T>
            {
                Id = Guid.Parse(ent.Id),
                Source = ent.Source,
                Version = ent.Version
            };
        }

        public async Task EnsureIndexExists(string index)
        {
            // TODO: make this nicer
            try
            {
                await _webClient.PutAsync(new Uri(IndexName(index), UriKind.Relative), new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            }
            catch
            {

            }
        }

        object MapType(Type type, bool nested)
        {
            var typeMappings = new Dictionary<Type, string>
            {
                { typeof(string), "text" },
                { typeof(Guid), "text" },
                { typeof(long), "long" },
                { typeof(int), "integer" },
                { typeof(short), "short" },
                { typeof(byte), "byte" },
                { typeof(double), "double" },
                { typeof(float), "float" },
                { typeof(DateTime), "date" },
                { typeof(bool), "boolean" },
            };

            var typeInfo = type.GetTypeInfo();
            if (nested && typeInfo.GetProperties().Any(ent => ent.Name == "Id"))
            {
                return new { type = "text" };
            }
            return new
            {
                properties = typeInfo.GetProperties().ToDictionary(ent => ent.Name, ent => (typeMappings.ContainsKey(ent.PropertyType) ? new
                {
                    type = typeMappings[ent.PropertyType]
                } : MapType(ent.PropertyType, true)))
            };
        }

        public async Task PutTypeMapping(string index, Type type)
        {
            var uri = new Uri($"{IndexName(index)}/_mapping/{type.FullName}", UriKind.Relative);
            var body = MapType(type, false);

            var json = JsonConvert.SerializeObject(body);

            _log.LogDebug($"{nameof(PutTypeMapping)} {type.FullName}");
            _log.LogDebug(json);

            var res = await _webClient.PutAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));

            _log.LogDebug(await res.Content.ReadAsStringAsync());
        }

        object ParseQuery(object query)
        {
            if (query == null) { return null; }

            if (query is string)
            {
                return new { query = new { query_string = query } };
            }

            return new { query };
        }

        Uri ItemUri<T>(string index, Guid id, Guid? parent = null, int? version = null, bool waitForRefresh = false)
            => ItemUri(typeof(T), index, id, parent, version, waitForRefresh);

        Uri ItemUri(Type type, string index, Guid id, Guid? parent = null, int? version = null, bool waitForRefresh = false)
        {
            var queryString = new Dictionary<string, object>();
            if (parent.HasValue)
            {
                queryString.Add("parent", parent);
            }
            if (version.HasValue)
            {
                queryString.Add("version", version);
            }
            if (waitForRefresh)
            {
                queryString.Add("refresh", "wait_for");
            }

            var path = $"{IndexName(index)}/{type.FullName}/{id}";

            return new Uri(path + queryString.ToQueryString(), UriKind.Relative);
        }

        string IndexName(string name)
            => string.IsNullOrEmpty(_settings.IndexPrefix) ? name : $"{_settings.IndexPrefix}_{name}";


        class SearchResult<TEnt>
        {
            [JsonProperty("took")]
            public int Took { get; set; }
            [JsonProperty("timed_out")]
            public bool TimedOut { get; set; }
            [JsonProperty("hits")]
            public SearchResultHits<TEnt> Hits { get; set; }
        }

        class SearchResultHits<TEnt>
        {
            [JsonProperty("total")]
            public int Total { get; set; }

            [JsonProperty("hits")]
            public IEnumerable<SearchResultHitsHit<TEnt>> Hits { get; set; }
        }

        class SearchResultHitsHit<TEnt>
        {
            [JsonProperty("_index")]
            public string Index { get; set; }
            [JsonProperty("_type")]
            public string Type { get; set; }
            [JsonProperty("_id")]
            public string Id { get; set; }
            [JsonProperty("_score")]
            public double Score { get; set; }
            [JsonProperty("_version")]
            public int Version { get; set; }
            [JsonProperty("_source")]
            public TEnt Source { get; set; }
        }
    }

    // TODO: replace this with a framework function that does this
    static class QueryStringHelpers
    {
        public static string ToQueryString(this Dictionary<string, object> dict, bool excludeQuestionMark = false)
        {
            var ret = string.Join("&", dict.Select(ent => $"{ent.Key}={ent.Value}"));

            return excludeQuestionMark ? ret : $"?{ret}";
        }
    }
}
