using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess;
using Kasbah.Content.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Kasbah.Content
{
    public class ContentService
    {
        const string IndexName = "nodes";
        readonly IDataAccessProvider _dataAccessProvider;
        readonly ILogger _log;
        public ContentService(IDataAccessProvider dataAccessProvider, ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<ContentService>();
            _dataAccessProvider = dataAccessProvider;
        }

        #region Public methods

        public async Task<Guid> CreateNodeAsync(Guid? parent, string alias, string displayName, string type)
        {
            var id = Guid.NewGuid();
            var node = new Node
            {
                Id = id,
                Parent = parent,
                Alias = alias,
                DisplayName = displayName,
                Type = type,
                Taxonomy = await CalculateTaxonomyAsync(parent, id, alias)
            };

            await _dataAccessProvider.PutEntryAsync(IndexName, id, node);

            return id;
        }

        public async Task<IEnumerable<Node>> ListChildrenAsync(Guid? parent)
        {
            object query = null;

            if (parent.HasValue)
            {
                query = new
                {
                    term = new
                    {
                        Parent = parent.Value.ToString()
                    }
                };
            }
            else
            {
                query = new
                {
                    @bool = new
                    {
                        must_not = new
                        {
                            exists = new
                            {
                                field = "Parent"
                            }
                        }
                    }
                };
            }

            var entries = await _dataAccessProvider.QueryEntriesAsync<Node>(IndexName, query);

            return entries.Select(ent => ent.Entry);
        }

        public async Task<IDictionary<string, object>> GetRawDataAsync(Guid id)
        {
            var node = await GetNodeAsync(id);

            return node.Data;
        }

        public async Task<T> GetTypedDataAsync<T>(Guid id)
        {
            var data = await GetRawDataAsync(id);

            // TODO: implement mapping dictionary > object

            throw new NotImplementedException();
        }

        public async Task UpdateDataAsync(Guid id, IDictionary<string, object> data)
        {
            var node = await GetNodeAsync(id);

            node.Data = data;

            await _dataAccessProvider.PutEntryAsync(IndexName, id, node);
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(ContentService)}");
            await _dataAccessProvider.EnsureIndexExists(IndexName);
        }

        #endregion

        #region Private methods

        async Task<Node> GetNodeAsync(Guid id)
        {
            return await _dataAccessProvider.GetEntryAsync<Node>(IndexName, id);
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

        #endregion
    }
}