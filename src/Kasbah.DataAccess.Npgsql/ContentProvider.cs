using System;
using System.Threading.Tasks;
using Kasbah.Content;
using Npgsql;
using Dapper;
using Kasbah.Content.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Kasbah.DataAccess.Npgsql
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
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, splitOn: "Ids");
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
                var ret = await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, new { id }, splitOn: "Ids");

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
                var ret = await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, new { aliases = aliases.ToArray() }, splitOn: "Ids");

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
                var ret = await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, new { ids = ids.ToArray() }, splitOn: "Ids");

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
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, new { types = types.ToArray() }, splitOn: "Ids");
            }
        }

        public async Task<IDictionary<string, object>> GetRawDataAsync(Guid id, long? version = default(long?))
        {
            const string Sql = "select content from node_content where id = @id and ((@version is not null and version = @version) or (@version is null and version = (select max(version) from node_content where id = @id)));";

            using (var connection = GetConnection())
            {
                var json = await connection.QueryFirstOrDefaultAsync<string>(Sql, new { id, version });

                if (json == null) { return null; }

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
                        await connection.ExecuteAsync("update node set published_version_id = @version where id = @id", new { id, version });
                    }

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task UpdateNodeAliasAsync(Guid id, string alias)
        {
            var node = await GetNodeAsync(id);
            var taxoIndex = node.Taxonomy.Aliases.Count();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    const string UpdateNodeSql = "update node set alias = @alias where id = @id";
                    var updateTaxonomySql = $"update node set alias_taxonomy[{taxoIndex}] = @alias where id_taxonomy[1:{taxoIndex}] = @taxonomy::uuid[]";

                    await connection.ExecuteAsync(UpdateNodeSql, new { id, alias });
                    await connection.ExecuteAsync(updateTaxonomySql, new { alias, taxonomy = node.Taxonomy.Ids });

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task ChangeNodeTypeAsync(Guid id, string type)
        {
            const string Sql = "update node set type = @type where id = @id";
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
                var deleteSql = $"delete from node where id_taxonomy[1:{taxoIndex}] = (select id_taxonomy from node where id = @id)";

                await connection.ExecuteAsync(deleteSql, new { id });
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
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, splitOn: "Ids");
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
                        id_taxonomy = (select id_taxonomy from node where id = @parent) || id
                        where id = @id";
                    var updateTaxonomySql = $"update node set alias_taxonomy = (select alias_taxonomy from node where id = @id) || alias, id_taxonomy = (select id_taxonomy from node where id = @id) || id where id_taxonomy[1:{taxoIndex}] = @taxonomy::uuid[]";

                    await connection.ExecuteAsync(UpdateNodeSql, new { id, parent, alias = node.Alias });
                    await connection.ExecuteAsync(updateTaxonomySql, new { id, taxonomy = node.Taxonomy.Ids });

                    await transaction.CommitAsync();
                }
            }
        }

        NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_settings.ConnectionString);
        }
    }
}
