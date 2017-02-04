using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kasbah.Content.Models;
using Kasbah.Media.Models;
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

        public async Task<long> PutEntryAsync<T>(string index, Guid id, T data, Guid? parent = null, bool waitForCommit = true)
        {
            var json = JsonConvert.SerializeObject(data);

            var response = await _webClient.PutAsync(ItemUri<T>(index, id, parent, waitForRefresh: waitForCommit), new StringContent(json, Encoding.UTF8, "application/json"));
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseJson);

            return (long)responseData["_version"];
        }

        public async Task<long> PutEntryAsync<T>(string index, Guid id, IDictionary<string, object> data, Guid? parent = null, bool waitForCommit = true)
            => await PutEntryAsync(index, id, typeof(T), data, parent, waitForCommit);

        public async Task<long> PutEntryAsync(string index, Guid id, Type type, IDictionary<string, object> data, Guid? parent = null, bool waitForCommit = true)
        {
            if (data?.ContainsKey("_version") == true)
            {
                data.Remove("_version");
            }
            var json = JsonConvert.SerializeObject(data);

            var response = await _webClient.PutAsync(ItemUri(type, index, id, parent, waitForRefresh: waitForCommit), new StringContent(json, Encoding.UTF8, "application/json"));
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseJson);

            return (long)responseData["_version"];
        }

        public async Task DeleteEntryAsync<T>(string index, Guid id)
        {
            _log.LogDebug($"{nameof(DeleteEntryAsync)}");
            var response = await _webClient.DeleteAsync(ItemUri<T>(index, id));
            _log.LogDebug($"{nameof(DeleteEntryAsync)}: {await response.Content.ReadAsStringAsync()}");
        }

        public async Task<IEnumerable<EntryWrapper<T>>> QueryEntriesAsync<T>(string index, object query = null, int? skip = 0, int? take = 10)
        {
            var queryObj = ParseQuery(query, skip, take);
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

        public async Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, long? version = null)
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

        public async Task<EntryWrapper<T>> GetEntryAsync<T>(string index, Guid id, Type type, long? version = null)
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

        public async Task EnsureIndexExistsAsync(string index)
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

        public async Task PutTypeMappingAsync(string index, Type type)
            => await PutTypeMappingAsync(index, type, MapType(type, false));

        public async Task PutTypeMappingAsync(string index, Type type, object mapping)
        {
            var uri = new Uri($"{IndexName(index)}/_mapping/{type.FullName}", UriKind.Relative);

            var json = JsonConvert.SerializeObject(mapping);

            _log.LogDebug($"{nameof(PutTypeMappingAsync)} {type.FullName}");
            _log.LogDebug(json);

            var res = await _webClient.PutAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));

            var resJson = await res.Content.ReadAsStringAsync();

            _log.LogDebug(resJson);

            var resObj = JsonConvert.DeserializeObject<PutTypeMappingResult>(resJson);
            if (!resObj.Acknowledged)
            {
                throw new InvalidOperationException($"Type mapping for {type.Name} on index {index} failed.\n{resJson}");
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

            var excludedProperties = new[] {
                "Id",
                "Node",
                "Version"
            };

            var typeInfo = type.GetTypeInfo();
            if (nested && (typeof(Item).IsAssignableFrom(type) || typeof(MediaItem) == type))
            {
                return new { type = "text" };
            }
            if (nested && (typeof(IEnumerable<Item>).IsAssignableFrom(type) || typeof(IEnumerable<MediaItem>) == type))
            {
                return new { type = "string" };
            }

            return new
            {
                properties = typeInfo.GetProperties()
                    .Where(ent => !excludedProperties.Contains(ent.Name))
                    .ToDictionary(ent => ent.Name, ent => (typeMappings.ContainsKey(ent.PropertyType) ? new
                    {
                        type = typeMappings[ent.PropertyType]
                    } : MapType(ent.PropertyType, true)))
            };
        }

        object ParseQuery(object query, int? skip = null, int? take = null)
        {
            var ret = new Dictionary<string, object>();

            if (query != null)
            {
                if (query is string)
                {
                    ret["query"] = new { query_string = query };
                }
                else
                {
                    ret["query"] = query;
                }
            }

            if (skip.HasValue)
            {
                ret["from"] = skip;
            }

            if (take.HasValue)
            {
                ret["size"] = take;
            }

            return ret;
        }

        Uri ItemUri<T>(string index, Guid id, Guid? parent = null, long? version = null, bool waitForRefresh = false)
            => ItemUri(typeof(T), index, id, parent, version, waitForRefresh);

        Uri ItemUri(Type type, string index, Guid id, Guid? parent = null, long? version = null, bool waitForRefresh = false)
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

        class PutTypeMappingResult
        {
            public bool Acknowledged { get; set; }
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
