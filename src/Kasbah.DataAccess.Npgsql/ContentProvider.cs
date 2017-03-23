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
    type in @types
";

            using (var connection = GetConnection())
            {
                return await connection.QueryAsync<Node, NodeTaxonomy, Node>(Sql, (node, taxonomy) =>
                {
                    node.Taxonomy = taxonomy;

                    return node;
                }, new { types }, splitOn: "Ids");
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

        NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_settings.ConnectionString);
        }
    }
}
