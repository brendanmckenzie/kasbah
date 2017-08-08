using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kasbah.Content;
using Kasbah.Content.Models;
using Newtonsoft.Json;
using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    public class ContentProvider : IContentProvider
    {
        readonly NpgsqlSettings _settings;

        public ContentProvider(NpgsqlSettings settings)
        {
            _settings = settings;
        }

        public async Task<Guid> CreateNodeAsync(Guid? parent, string alias, string type, string displayName = null)
        {
            const string Sql = @"
insert into node
( id, parent_id, alias, type, display_name,
    id_taxonomy,
    alias_taxonomy )
values
( @id, @parent, @alias, @type, @displayName,
    (select id_taxonomy from node where id = @parent) || @id::uuid,
    (select alias_taxonomy from node where id = @parent) || @alias::varchar(512)
);";
            using (var connection = GetConnection())
            {
                var id = Guid.NewGuid();

                await connection.ExecuteAsync(Sql, new { id, parent, alias, type, displayName });

                return id;
            }
        }

        public async Task<IEnumerable<Node>> DescribeTreeAsync()
        {
            const string Sql = @"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
";

            using (var connection = GetConnection())
            {
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, NodeMapper, splitOn: "Ids");
            }
        }

        public async Task<Node> GetNodeAsync(Guid id)
        {
            const string Sql = @"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
where
    id = @id
";

            using (var connection = GetConnection())
            {
                var ret = await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, NodeMapper, new { id }, splitOn: "Ids");

                return ret.SingleOrDefault();
            }
        }

        public async Task<Node> GetNodeByTaxonomy(IEnumerable<string> aliases)
        {
            const string Sql = @"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
where
    alias_taxonomy = @aliases::varchar(512)[]
";

            using (var connection = GetConnection())
            {
                var ret = await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, NodeMapper, new { aliases = aliases.ToArray() }, splitOn: "Ids");

                return ret.SingleOrDefault();
            }
        }

        public async Task<Node> GetNodeByTaxonomy(IEnumerable<Guid> ids)
        {
            const string Sql = @"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
where
    ids_taxonomy = @ids::uuid[]
";

            using (var connection = GetConnection())
            {
                var ret = await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, NodeMapper, new { ids = ids.ToArray() }, splitOn: "Ids");

                return ret.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<Node>> GetNodesByType(IEnumerable<string> types)
        {
            const string Sql = @"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
where
    type = any(@types)
";

            using (var connection = GetConnection())
            {
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, NodeMapper, new { types = types.ToArray() }, splitOn: "Ids");
            }
        }

        public async Task<IDictionary<string, object>> GetRawDataAsync(Guid id, long? version = default(long?))
        {
            const string Sql = "select content from node_content where id = @id and ((@version is not null and version = @version) or (@version is null and version = (select max(version) from node_content where id = @id)));";

            using (var connection = GetConnection())
            {
                var json = await connection.QueryFirstOrDefaultAsync<string>(Sql, new { id, version });

                if (json == null)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
        }

        public async Task UpdateDataAsync(Guid id, IDictionary<string, object> data, bool publish)
        {
            const string Sql = "insert into node_content ( id, version, content ) values ( @id, (select coalesce(max(version), 0) + 1 from node_content where id = @id), @content::jsonb ) returning version;";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var version = await connection.QuerySingleAsync<int>(Sql, new { id, content = JsonConvert.SerializeObject(data) });

                    if (publish)
                    {
                        await connection.ExecuteAsync("update node set published_version_id = @version, modified_at = now() where id = @id", new { id, version });
                    }

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task UpdateNodeAliasAsync(Guid id, string alias)
        {
            // TODO: check for clashes
            var node = await GetNodeAsync(id);
            var taxoIndex = node.Taxonomy.Aliases.Count();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    const string UpdateNodeSql = "update node set alias = @alias, modified_at = now() where id = @id";
                    var updateTaxonomySql = $"update node set alias_taxonomy[{taxoIndex}] = @alias, modified_at = now() where id_taxonomy[1:{taxoIndex}] = @taxonomy::uuid[]";

                    await connection.ExecuteAsync(UpdateNodeSql, new { id, alias });
                    await connection.ExecuteAsync(updateTaxonomySql, new { alias, taxonomy = node.Taxonomy.Ids });

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task ChangeNodeTypeAsync(Guid id, string type)
        {
            const string Sql = "update node set type = @type, modified_at = now() where id = @id";
            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync(Sql, new { id, type });
            }
        }

        public async Task DeleteNodeAsync(Guid id)
        {
            var node = await GetNodeAsync(id);

            // TODO: this shouldn't be required
            var taxoIndex = node.Taxonomy.Aliases.Count();

            using (var connection = GetConnection())
            {
                var deleteContentSql = $"delete from node_content where id in (select id from node where id_taxonomy[1:{taxoIndex}] = (select id_taxonomy from node where id = @id))";
                var deleteNodesSql = $"delete from node where id_taxonomy[1:{taxoIndex}] = (select id_taxonomy from node where id = @id)";

                await connection.ExecuteAsync(deleteContentSql, new { id });
                await connection.ExecuteAsync(deleteNodesSql, new { id });
            }
        }

        public async Task<IEnumerable<Node>> GetRecentlyModified(int take)
        {
            var sql = $@"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
order by
    modified_at desc
limit {take}";

            using (var connection = GetConnection())
            {
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(sql, NodeMapper, splitOn: "Ids");
            }
        }

        public async Task MoveNodeAsync(Guid id, Guid? parent)
        {
            var node = await GetNodeAsync(id);
            var taxoIndex = node.Taxonomy.Aliases.Count();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    const string UpdateNodeSql = @"
                    update node set
                        parent_id = @parent,
                        alias_taxonomy = (select alias_taxonomy from node where id = @parent) || alias,
                        id_taxonomy = (select id_taxonomy from node where id = @parent) || id,
                        modified_at = now()
                        where id = @id";
                    var updateTaxonomySql = $"update node set alias_taxonomy = (select alias_taxonomy from node where id = @id) || alias, id_taxonomy = (select id_taxonomy from node where id = @id) || id where id_taxonomy[1:{taxoIndex}] = @taxonomy::uuid[]";

                    await connection.ExecuteAsync(UpdateNodeSql, new { id, parent, alias = node.Alias });
                    await connection.ExecuteAsync(updateTaxonomySql, new { id, taxonomy = node.Taxonomy.Ids });

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task<Node> PutNodeAsync(Node node)
        {
            var currentNode = await GetNodeAsync(node.Id);

            if (!string.Equals(currentNode.Alias, node.Alias))
            {
                await UpdateNodeAliasAsync(node.Id, node.Alias);
            }

            if (!string.Equals(currentNode.Type, node.Type))
            {
                await ChangeNodeTypeAsync(node.Id, node.Type);
            }

            if (!string.Equals(currentNode.DisplayName, node.DisplayName))
            {
                const string Sql = @"update node set display_name = @DisplayName, modified_at = now() where id = @Id";
                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(Sql, node);
                }
            }

            return node;
        }

        public async Task<IEnumerable<Node>> ListNodesByParentAsync(Guid? parent)
        {
            const string Sql = @"
select
    id as Id,
    parent_id as Parent,
    alias as Alias,
    display_name as DisplayName,
    type as Type,
    published_version_id as PublishedVersion,
    created_at as Created,
    modified_at as Modified,
    id_taxonomy as Ids,
    alias_taxonomy as Aliases
from
    node
where
    (@parent is null and parent_id is null)
    or (parent_id = @parent_id)
";

            using (var connection = GetConnection())
            {
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, NodeMapper, new { parent }, splitOn: "Ids");
            }
        }

        Node NodeMapper(Node node, NodeTaxonomy taxonomy)
        {
            node.Taxonomy = taxonomy;
            node.Children = new Lazy<IEnumerable<Node>>(() => ListNodesByParentAsync(node.Id).Result);

            return node;
        }

        NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_settings.ConnectionString);
        }
    }
}
