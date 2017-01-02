using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess;
using Kasbah.Content.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Net.Http;

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
        readonly TypeRegistry _typeRegistry;
        public ContentService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider, TypeRegistry typeRegistry)
        {
            _log = loggerFactory.CreateLogger<ContentService>();
            _dataAccessProvider = dataAccessProvider;
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
                Taxonomy = await CalculateTaxonomyAsync(parent, id, alias)
            };

            await _dataAccessProvider.PutEntryAsync(Indicies.Nodes, id, node);

            return id;
        }

        public async Task<IEnumerable<Node>> DescribeTreeAsync()
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<Node>(Indicies.Nodes);

            return entries.Select(ent => ent.Entry);
        }

        public async Task<IDictionary<string, object>> GetRawDataAsync(Guid id)
        {
            try
            {
                var nodeData = await _dataAccessProvider.GetEntryAsync<NodeData>(Indicies.Content, id);

                return nodeData.Data;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<T> GetTypedDataAsync<T>(Guid id)
        {
            var data = await GetRawDataAsync(id);

            // TODO: implement mapping dictionary > object

            throw new NotImplementedException();
        }

        public async Task UpdateDataAsync(Guid id, IDictionary<string, object> data)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Content, id, new NodeData { Data = data });
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

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(ContentService)}");
            await _dataAccessProvider.EnsureIndexExists(Indicies.Nodes);
            await _dataAccessProvider.EnsureIndexExists(Indicies.Content);
        }

        #endregion

        #region Private methods

        async Task<Node> GetNodeAsync(Guid id)
        {
            return await _dataAccessProvider.GetEntryAsync<Node>(Indicies.Nodes, id);
        }

        async Task<NodeTaxonomy> CalculateTaxonomyAsync(Guid? parent, Guid id, string alias)
        {
            if (!parent.HasValue)
            {
                return new NodeTaxonomy
                {
                    Ids = new[] { id },
                    Aliases = new[] { alias }
                };
            }
            else
            {
                var node = await GetNodeAsync(parent.Value);

                return new NodeTaxonomy
                {
                    Ids = node.Taxonomy.Ids.Concat(new[] { id }),
                    Aliases = node.Taxonomy.Aliases.Concat(new[] { alias })
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

        #endregion
    }
}


// public async Task<IEnumerable<Node>> ListChildrenAsync(Guid? parent)
// {
//     object query = null;

//     if (parent.HasValue)
//     {
//         query = new
//         {
//             term = new
//             {
//                 Parent = parent.Value.ToString()
//             }
//         };
//     }
//     else
//     {
//         query = new
//         {
//             @bool = new
//             {
//                 must_not = new
//                 {
//                     exists = new
//                     {
//                         field = "Parent"
//                     }
//                 }
//             }
//         };
//     }

//     var entries = await _dataAccessProvider.QueryEntriesAsync<Node>(Indicies.Nodes, query);

//     return entries.Select(ent => ent.Entry);
// }