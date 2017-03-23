using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content.Models;
using Kasbah.Extensions;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using Kasbah.Exceptions;
using Kasbah.Content.Events;

namespace Kasbah.Content
{
    public class ContentService
    {
        readonly ILogger _log;
        readonly IContentProvider _contentProvider;
        readonly IDistributedCache _cache;
        readonly TypeRegistry _typeRegistry;
        readonly EventBus _eventBus;

        public ContentService(ILoggerFactory loggerFactory, IContentProvider contentProvider, TypeRegistry typeRegistry, EventBus eventBus, IDistributedCache cache = null)
        {
            _log = loggerFactory.CreateLogger<ContentService>();
            _contentProvider = contentProvider;
            _typeRegistry = typeRegistry;
            _eventBus = eventBus;
            _cache = cache;
        }

        #region Public methods

        public async Task<Guid> CreateNodeAsync(Guid? parent, string alias, string type, string displayName = null)
        {
            await CheckCanCreateNodeAsync(parent, alias, type);

            var id = await _contentProvider.CreateNodeAsync(parent, alias, type, displayName);

            await _cache?.RemoveAsync(nameof(DescribeTreeAsync));

            return id;
        }

        public async Task<IEnumerable<Node>> DescribeTreeAsync()
            => await _contentProvider.DescribeTreeAsync();

        public async Task<IDictionary<string, object>> GetRawDataAsync(Guid id, long? version = null)
        {
            try
            {
                return await _cache.GetOrSetAsync($"{nameof(GetRawDataAsync)}_{id}_{version}", async () =>
                {
                    return await _contentProvider.GetRawDataAsync(id, version);
                });
            }
            catch (EntryNotFoundException)
            {
                return null;
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
                Data = data,
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

        public async Task<IEnumerable<Node>> GetChildrenAsync(Guid? parent)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

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
        {
            // TODO: optimise
            var tree = await DescribeTreeAsync();

            return tree.OrderByDescending(ent => ent.Modified).Take(take);
        }

        public async Task<long> DeleteNodeAsync(Guid id)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public async Task UpdateNodeAliasAsync(Guid id, string alias)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public async Task<IEnumerable<ContentPatch>> ListContentPatchesAsync(Guid id)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        #endregion

        #region Private methods

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

        #endregion
    }
}
