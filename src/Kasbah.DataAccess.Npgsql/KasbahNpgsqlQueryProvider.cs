using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kasbah.DataAccess.Npgsql
{
    class KasbahNpgsqlQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(KasbahNpgsqlQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new KasbahNpgsqlQueryable<TElement>(this, expression);

        public object Execute(Expression expression)
        {
            var translator = new KasbahQueryTranslator();
            var sql = translator.Translate(expression);
            
            throw new NotImplementedException($"Did it work? {sql}");
            /*var types = _typeRegistry.GetTypesThatImplement<TItem>();

            // 1. get list of known types that implement provided TItem
            // 2. build SQL query
            // 3. get results

            using (var connection = GetConnection())
            {
                var sql = @"
                select 
                    node.*, 
                    node_content.* 
                from 
                    node
                    inner join node_content on (node.published_version_id = node_content.id)
                where
                    node.type in @types";
                // await connection.QueryAsync(sql, new  { types });
            }*/
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