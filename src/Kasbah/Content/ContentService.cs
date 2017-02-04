using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess;
using Kasbah.Content.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Reflection;

namespace Kasbah.Content
{
    public class ContentService
    {
        static class Indicies
        {
            public const string Nodes = "nodes";
            public const string Content = "content";
        }

        readonly ILogger _log;
        readonly IDataAccessProvider _dataAccessProvider;
        readonly IDistributedCache _cache;
        readonly TypeRegistry _typeRegistry;

        public ContentService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider, TypeRegistry typeRegistry, IDistributedCache cache = null)
        {
            _log = loggerFactory.CreateLogger<ContentService>();
            _dataAccessProvider = dataAccessProvider;
            _cache = cache;
            _typeRegistry = typeRegistry;
        }

        #region Public methods

        public async Task<Guid> CreateNodeAsync(Guid? parent, string alias, string type, string displayName = null)
        {
            await CheckCanCreateNodeAsync(parent, alias, type);

            var id = Guid.NewGuid();
            var node = new Node
            {
                Id = id,
                Parent = parent,
                Alias = alias,
                DisplayName = displayName ?? alias,
                Type = type,
                Taxonomy = await CalculateTaxonomyAsync(parent, id, alias),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            await _dataAccessProvider.PutEntryAsync(Indicies.Nodes, id, node);

            await _cache.RemoveAsync(nameof(DescribeTreeAsync));

            return id;
        }

        public async Task<IEnumerable<Node>> DescribeTreeAsync()
        {
            return await CacheGetOrSet(nameof(DescribeTreeAsync), async () =>
            {
                var entries = await _dataAccessProvider.QueryEntriesAsync<Node>(Indicies.Nodes, take: 1024);

                return entries.Select(ent => ent.Source);
            });
        }

        public async Task<IDictionary<string, object>> GetRawDataAsync(Guid id, long? version = null)
        {
            try
            {
                return await CacheGetOrSet($"{nameof(GetRawDataAsync)}_{id}_{version}", async () =>
                {
                    var node = await GetNodeAsync(id);
                    var type = Type.GetType(node.Type);

                    var data = await _dataAccessProvider.GetEntryAsync<IDictionary<string, object>>(Indicies.Content, id, type, version);

                    var res = data.Source;
                    res["_version"] = data.Version;

                    return res;
                });
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task UpdateDataAsync(Guid id, IDictionary<string, object> data, bool publish)
        {
            var node = await GetNodeAsync(id);
            var type = Type.GetType(node.Type);

            var version = await _dataAccessProvider.PutEntryAsync(Indicies.Content, id, type, data);

            await _cache.RemoveAsync($"{nameof(GetRawDataAsync)}_{id}_{null}");

            node.Modified = DateTime.UtcNow;
            if (publish)
            {
                node.PublishedVersion = version;
            }

            await UpdateNodeAsync(id, node);
        }

        // TODO: could probably use better naming conventions
        public async Task<NodeDataForEditing> GetNodeDataForEditingAsync(Guid id)
        {
            var node = await GetNodeAsync(id);
            var data = await GetRawDataAsync(id);
            var type = _typeRegistry.GetType(node.Type);

            return new NodeDataForEditing
            {
                Node = node,
                Data = data,
                Type = type
            };
        }

        public async Task<Node> GetNodeByTaxonomy(IEnumerable<string> aliases)
        {
            var key = $"{nameof(GetNodeByTaxonomy)}_{string.Join("_", aliases)}";
            return await CacheGetOrSet(key, async () =>
            {
                var query = new
                {
                    @bool = new
                    {
                        must = aliases.Select(ent => new QueryMatch
                        {
                            Match = new Dictionary<string, string> {
                                { "Taxonomy.Aliases", ent }
                            }
                        }).Concat(new[] { new QueryMatch {
                            Match = new Dictionary<string, int> {
                                { "Taxonomy.Length", aliases.Count() }
                            }
                        }})
                    }
                };
                var items = await _dataAccessProvider.QueryEntriesAsync<Node>(Indicies.Nodes, query);

                return items.Select(ent => ent.Source).FirstOrDefault();
            });
        }

        public async Task<Node> GetNodeByTaxonomy(IEnumerable<Guid> ids)
        {
            var key = $"{nameof(GetNodeByTaxonomy)}_{string.Join("_", ids)}";
            return await CacheGetOrSet(key, async () =>
            {
                var query = new
                {
                    @bool = new
                    {
                        must = ids.Select(ent => new QueryMatch
                        {
                            Match = new Dictionary<string, Guid> {
                            { "Taxonomy.Ids", ent }
                        }
                        }).Concat(new[] { new QueryMatch {
                            Match = new Dictionary<string, int> {
                                { "Taxonomy.Length", ids.Count() }
                            }
                        }})
                    }
                };
                var items = await _dataAccessProvider.QueryEntriesAsync<Node>(Indicies.Nodes, query);

                return items.Select(ent => ent.Source).FirstOrDefault();
            });
        }

        public async Task<Node> GetChildByAliasAsync(Guid? parent, string alias)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public async Task<IEnumerable<Node>> GetChildrenAsync(Guid? parent)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(ContentService)}");
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Nodes);
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Content);

            await UpdateMappingsAsync();
        }

        public async Task<Node> GetNodeAsync(Guid id)
        {
            return await CacheGetOrSet($"{nameof(GetNodeAsync)}_{id}", async () =>
            {
                return (await _dataAccessProvider.GetEntryAsync<Node>(Indicies.Nodes, id)).Source;
            });
        }

        public async Task<IEnumerable<Node>> GetNodesByType(string type, bool inherit = false)
        {
            var typesToQuery = new[] { type }.AsEnumerable();

            if (inherit)
            {
                var typeObj = Type.GetType(type);
                typesToQuery = typesToQuery
                    .Concat(_typeRegistry.ListTypes()
                        .Where(ent => typeObj.GetTypeInfo().IsAssignableFrom(Type.GetType(ent.Alias)))
                        .Select(ent => ent.Alias));

            }

            var query = new
            {
                terms = new
                {
                    Type = typesToQuery
                }
            };

            var items = await _dataAccessProvider.QueryEntriesAsync<Node>(Indicies.Nodes, query);

            return items.Select(ent => ent.Source);
        }

        #endregion

        #region Private methods

        async Task UpdateNodeAsync(Guid id, Node node)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Nodes, id, node);

            await _cache.RemoveAsync($"{nameof(GetNodeAsync)}_{id}");
            await _cache.RemoveAsync($"{nameof(GetNodeByTaxonomy)}_{string.Join("_", node.Taxonomy.Aliases)}");
        }

        async Task<NodeTaxonomy> CalculateTaxonomyAsync(Guid? parent, Guid id, string alias)
        {
            if (!parent.HasValue)
            {
                return new NodeTaxonomy
                {
                    Ids = new[] { id },
                    Aliases = new[] { alias },
                    Length = 1
                };
            }
            else
            {
                var node = await GetNodeAsync(parent.Value);

                return new NodeTaxonomy
                {
                    Ids = node.Taxonomy.Ids.Concat(new[] { id }),
                    Aliases = node.Taxonomy.Aliases.Concat(new[] { alias }),
                    Length = node.Taxonomy.Aliases.Count() + 1
                };
            }
        }

        async Task CheckCanCreateNodeAsync(Guid? parent, string alias, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (_typeRegistry.GetType(type) == null)
            {
                throw new InvalidOperationException($"Unknown type {type}. Did you register it with the {nameof(TypeRegistry)}?");
            }
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }

            // TODO: make this more efficient, create a GetChildByAlias() function or something
            var tree = await DescribeTreeAsync();
            if (tree.Any(ent => ent.Alias == alias && ent.Parent == parent)) { throw new InvalidOperationException($"Node with alias {alias} already exists under {parent}"); }
        }

        async Task UpdateMappingsAsync()
        {
            await UpdateNodeMappingAsync();

            await Task.WhenAll(
                _typeRegistry.ListTypes()
                    .Select(ent => _dataAccessProvider.PutTypeMappingAsync(Indicies.Content, Type.GetType(ent.Alias)))
            );
        }

        async Task UpdateNodeMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(Node.Id), new { type = "keyword" } },
                { nameof(Node.Parent), new { type = "keyword" } },
                { nameof(Node.Taxonomy), new {
                    properties = new Dictionary<string, object> {
                        { nameof(NodeTaxonomy.Ids), new { type = "keyword" } },
                        { nameof(NodeTaxonomy.Aliases), new { type = "keyword" } },
                        { nameof(NodeTaxonomy.Length), new { type = "integer" } }
                    }
                 } },
                { nameof(Node.Alias), new { type = "keyword" } },
                { nameof(Node.Type), new { type = "keyword" } },
                { nameof(Node.DisplayName), new { type = "text" } },
                { nameof(Node.PublishedVersion), new { type = "integer" } },
                { nameof(Node.Created), new { type = "date" } },
                { nameof(Node.Modified), new { type = "date" } }
            };

            await _dataAccessProvider.PutTypeMappingAsync(Indicies.Nodes, typeof(Node), new { properties });
        }

        async Task<T> CacheGetOrSet<T>(string key, Func<Task<T>> generator)
        {
            // TODO: find out why GetStringAsync/SetStringAsync don't work
            var cacheValue = _cache.GetString(key);
            if (cacheValue == null)
            {
                _log.LogDebug($"Cache miss: {key}");
                var generatedValue = await generator();

                _cache.SetString(key, JsonConvert.SerializeObject(generatedValue));

                return generatedValue;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(cacheValue);
            }
        }

        #endregion

        class QueryMatch
        {
            [JsonProperty("match")]
            public object Match { get; set; }
        }
    }
}
