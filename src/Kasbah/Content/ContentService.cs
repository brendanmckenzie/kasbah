using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Content.Events;
using Kasbah.Content.Models;
using Kasbah.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Profiling;

namespace Kasbah.Content
{
    public class ContentService
    {
        readonly ILogger _log;
        readonly IContentProvider _contentProvider;
        readonly IDistributedCache _cache;
        readonly TypeRegistry _typeRegistry;
        readonly EventBus _eventBus;
        readonly IKasbahQueryProviderFactory _queryProviderFactory;

        public ContentService()
        {
            // TODO: find out how to not need this. it's here because of unit tests
        }

        public ContentService(ILoggerFactory loggerFactory, IContentProvider contentProvider, TypeRegistry typeRegistry, EventBus eventBus, IKasbahQueryProviderFactory queryProviderFactory, IDistributedCache cache = null)
        {
            _log = loggerFactory.CreateLogger<ContentService>();
            _contentProvider = contentProvider;
            _typeRegistry = typeRegistry;
            _eventBus = eventBus;
            _cache = cache;
            _queryProviderFactory = queryProviderFactory;
        }

        public async Task<Guid> CreateNodeAsync(Guid? parent, string alias, string type, string displayName = null)
        {
            await CheckCanCreateNodeAsync(parent, alias, type);

            var id = await _contentProvider.CreateNodeAsync(parent, alias, type, displayName);

            await _cache?.RemoveAsync(nameof(DescribeTreeAsync));

            return id;
        }

        public async Task<IEnumerable<Node>> DescribeTreeAsync()
            => await _contentProvider.DescribeTreeAsync();

        public async Task<string> GetRawDataAsync(Guid id, long? version = null)
        {
            using (MiniProfiler.Current.Step(nameof(GetRawDataAsync)))
            {
                return await _cache?.GetOrSetAsync($"{nameof(GetRawDataAsync)}_{id}_{version}", async () =>
                {
                    return await _contentProvider.GetRawDataAsync(id, version);
                });
            }
        }

        public async Task UpdateDataAsync(Guid id, IDictionary<string, object> data, bool publish)
        {
            var node = await GetNodeAsync(id);

            await _contentProvider.UpdateDataAsync(id, data, publish);

            await _cache?.RemoveAsync($"{nameof(GetRawDataAsync)}_{id}_{null}");
            if (publish)
            {
                await _eventBus.TriggerContentPublished(node);
            }

            await _cache?.RemoveAsync($"{nameof(GetNodeAsync)}_{id}");
            await _cache?.RemoveAsync($"{nameof(GetNodeByTaxonomy)}_{string.Join("_", node.Taxonomy.Aliases)}");
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
                Data = JsonConvert.DeserializeObject<IDictionary<string, object>>(data ?? "{}"),
                Type = type
            };
        }

        public async Task<Node> GetNodeByTaxonomy(IEnumerable<string> aliases)
            => await _contentProvider.GetNodeByTaxonomy(aliases);

        public async Task<Node> GetNodeByTaxonomy(IEnumerable<Guid> ids)
            => await _contentProvider.GetNodeByTaxonomy(ids);

        public async Task<Node> GetChildByAliasAsync(Guid? parent, string alias)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public async Task<IEnumerable<Node>> ListNodesByParentAsync(Guid? parent)
            => await _contentProvider.ListNodesByParentAsync(parent);

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(ContentService)}");

            await Task.Delay(0);
        }

        public async Task<Node> GetNodeAsync(Guid id)
            => await _contentProvider.GetNodeAsync(id);

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

            return await _contentProvider.GetNodesByType(typesToQuery);
        }

        public async Task<IEnumerable<Node>> GetRecentlyModified(int take)
            => await _contentProvider.GetRecentlyModified(take);

        public async Task DeleteNodeAsync(Guid id)
            => await _contentProvider.DeleteNodeAsync(id);

        public async Task UpdateNodeAliasAsync(Guid id, string alias)
            => await _contentProvider.UpdateNodeAliasAsync(id, alias);

        public async Task ChangeNodeTypeAsync(Guid id, string type)
            => await _contentProvider.ChangeNodeTypeAsync(id, type);

        public async Task MoveNodeAsync(Guid id, Guid? parent)
            => await _contentProvider.MoveNodeAsync(id, parent);

        public async Task<Node> PutNodeAsync(Node node)
            => await _contentProvider.PutNodeAsync(node);

        public async Task<IEnumerable<ContentPatch>> ListContentPatchesAsync(Guid id)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public IQueryable<TItem> Query<TItem>()
            where TItem : IItem
            => new KasbahQueryable<TItem>(_queryProviderFactory.CreateProvider(typeof(TItem)));

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
            if (tree.Any(ent => ent.Alias == alias && ent.Parent == parent))
            {
                throw new InvalidOperationException($"Node with alias {alias} already exists under {parent}");
            }
        }
    }
}
