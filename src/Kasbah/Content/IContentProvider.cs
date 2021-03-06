using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kasbah.Content.Models;

namespace Kasbah.Content
{
    public interface IContentProvider
    {
        Task<Guid> CreateNodeAsync(Guid? parent, string alias, string type, string displayName = null);

        Task<IEnumerable<Node>> DescribeTreeAsync();

        Task<string> GetRawDataAsync(Guid id, long? version = null);

        Task UpdateDataAsync(Guid id, IDictionary<string, object> data, bool publish);

        Task<Node> GetNodeByTaxonomy(IEnumerable<string> aliases);

        Task<Node> GetNodeByTaxonomy(IEnumerable<Guid> ids);

        Task<Node> GetNodeAsync(Guid id);

        Task<IEnumerable<Node>> GetNodesByType(IEnumerable<string> types);

        Task UpdateNodeAliasAsync(Guid id, string alias);

        Task ChangeNodeTypeAsync(Guid id, string type);

        Task DeleteNodeAsync(Guid id);

        Task<IEnumerable<Node>> GetRecentlyModified(int take);

        Task MoveNodeAsync(Guid id, Guid? parent);

        Task<Node> PutNodeAsync(Node node);

        Task<IEnumerable<Node>> ListNodesByParentAsync(Guid? parent);
    }
}
