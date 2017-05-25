using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Npgsql;
using Dapper;
using Kasbah.Content;
using Newtonsoft.Json;
using System.Collections;

namespace Kasbah.DataAccess.Npgsql
{
    class KasbahNpgsqlQueryProvider : IQueryProvider
    {
        readonly Type _targetType;
        readonly NpgsqlSettings _settings;
        readonly TypeRegistry _typeRegistry;
        readonly TypeMapper _typeMapper;
        public KasbahNpgsqlQueryProvider(Type targetType, NpgsqlSettings settings, TypeRegistry typeRegistry, TypeMapper typeMapper)
        {
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
            // TODO: fix issue specifying types parameter

            var types = _typeRegistry.GetTypesThatImplement(_targetType);

            var translator = new KasbahNpgsqlQueryTranslator();
            var res = translator.Translate(expression);

            var sql = new StringBuilder();
            sql.Append("select content from node_content nc inner join node n on nc.id = n.id where ");
            if (types.Any())
            {
                // sql.Append("(n.type in @types) and ");
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

            Console.WriteLine(sql);

            using (var connection = new NpgsqlConnection(_settings.ConnectionString))
            {
                var parameters = new Dapper.DynamicParameters(res.Parameters);
                if (types.Any())
                {
                    // parameters.Add("types", types.Select(ent => ent.Alias).ToArray());
                }
                Console.WriteLine($"parameters: {string.Join(", ", types.Select(ent => ent.Alias).ToArray())}");

                var rawData = connection.Query<string>(sql.ToString(), param: parameters);

                Console.WriteLine($"result count: {rawData.Count()}");

                var mappedData = rawData
                    .Select(json => JsonConvert.DeserializeObject<IDictionary<string, object>>(json))
                    .Select(ent => _typeMapper.MapTypeAsync(ent, _targetType.AssemblyQualifiedName).Result);

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

    internal static class IQueryableExtensions
    {
        public static string GetQueryText(this IQueryProvider provider, Expression expression)
        {
            return expression.ToString();
        }
    }

    internal static class TypeSystem
    {
        internal static Type GetElementType(Type type)
        {

            Type ienum = FindIEnumerable(type);

            if (ienum == null) return type;

            return ienum.GetGenericArguments()[0];

        }

        private static Type FindIEnumerable(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            if (type == null || type == typeof(string))
                return null;

            if (type.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());

            if (typeInfo.IsGenericType)
            {
                foreach (Type arg in type.GetGenericArguments())
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (ienum.IsAssignableFrom(type))
                    {
                        return ienum;
                    }
                }
            }

            var ifaces = type.GetInterfaces();

            if (ifaces.Any())
            {
                foreach (var iface in ifaces)
                {
                    var ienum = FindIEnumerable(iface);

                    if (ienum != null) return ienum;
                }
            }

            if (typeInfo.BaseType != null && typeInfo.BaseType != typeof(object))
            {
                return FindIEnumerable(typeInfo.BaseType);
            }

            return null;
        }
    }
}
