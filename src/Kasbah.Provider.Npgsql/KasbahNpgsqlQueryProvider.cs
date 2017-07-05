using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Kasbah.Content;
using Kasbah.Content.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    class KasbahNpgsqlQueryProvider : IQueryProvider
    {
        readonly ILogger _log;
        readonly Type _targetType;
        readonly NpgsqlSettings _settings;
        readonly TypeRegistry _typeRegistry;
        readonly TypeMapper _typeMapper;

        public KasbahNpgsqlQueryProvider(ILoggerFactory loggerFactory, Type targetType, NpgsqlSettings settings, TypeRegistry typeRegistry, TypeMapper typeMapper)
        {
            _log = loggerFactory.CreateLogger<KasbahNpgsqlQueryProvider>();
            _targetType = targetType;
            _settings = settings;
            _typeRegistry = typeRegistry;
            _typeMapper = typeMapper;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(KasbahQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new KasbahQueryable<TElement>(this, expression);

        public object Execute(Expression expression)
        {
            var translator = new KasbahNpgsqlQueryTranslator();
            var res = translator.Translate(expression);

            var sql = new StringBuilder();
            sql.Append(@"
select
    n.id as Id,
    n.parent_id as Parent,
    n.alias as Alias,
    n.display_name as DisplayName,
    n.type as Type,
    n.published_version_id as PublishedVersion,
    n.created_at as Created,
    n.modified_at as Modified,
    n.id_taxonomy as Ids,
    n.alias_taxonomy as Aliases,
    nc.content as Content
from
    node n
    inner join node_content nc on nc.id = n.id and n.published_version_id = nc.version
where ");
            var parameters = new Dapper.DynamicParameters(res.Parameters);

            var types = _typeRegistry.GetTypesThatImplement(_targetType);
            if (types.Any())
            {
                var typeWhere =
                    types.Select((ent, index) => new { Alias = ent.Alias, Index = index })
                    .Select(ent => new
                    {
                        Statement = $"n.type = @t{ent.Index}",
                        Key = $"t{ent.Index}",
                        Value = ent.Alias
                    });

                sql.Append($"({string.Join(" or ", typeWhere.Select(ent => ent.Statement))}) and ");

                // TODO: this still isn't the greatest
                foreach (var type in typeWhere)
                {
                    parameters.Add(type.Key, type.Value);
                }
            }

            sql.Append(string.IsNullOrEmpty(res.WhereClause) ? "1=1" : res.WhereClause);
            if (res.Take.HasValue)
            {
                sql.Append(" limit ");
                sql.Append(res.Take);
            }

            if (res.Skip.HasValue)
            {
                sql.Append(" offset ");
                sql.Append(res.Skip);
            }

            sql.Append(';');

            _log.LogDebug($"{nameof(Execute)}: {sql}");

            using (var connection = new NpgsqlConnection(_settings.ConnectionString))
            {
                var rawData = connection.Query<Node, string, QueryResult>(
                    sql: sql.ToString(),
                    map: (node, json) => new QueryResult { Node = node, Json = json },
                    param: parameters,
                    splitOn: "Content");

                var mappedData = rawData.Select(ent =>
                    {
                        var dict = JsonConvert.DeserializeObject<IDictionary<string, object>>(ent.Json);

                        return _typeMapper.MapTypeAsync(dict, ent.Node.Type, ent.Node, ent.Node.PublishedVersion).Result;
                    });

                // TODO: this isn't great
                var ret = Activator.CreateInstance(typeof(List<>).MakeGenericType(_targetType)) as IList;
                foreach (var ent in mappedData)
                {
                    ret.Add(ent);
                }

                return ret;
            }
        }

        public TResult Execute<TResult>(Expression expression)
            => (TResult)Execute(expression);
    }
}
