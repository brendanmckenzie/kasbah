using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kasbah.DataAccess.Npgsql
{
    class KasbahNpgsqlQueryable<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        readonly IQueryProvider _provider;
        readonly Expression _expression;

        public KasbahNpgsqlQueryable(IQueryProvider provider)
        {
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public KasbahNpgsqlQueryable(IQueryProvider provider, Expression expression)
        {
            _provider = provider;
            _expression = expression;
        }

        public Type ElementType
            => typeof(T);

        public Expression Expression
            => _expression;

        public IQueryProvider Provider
            => _provider;

        public IEnumerator<T> GetEnumerator()
            => ((IEnumerable<T>)_provider.Execute(_expression)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();


        public override string ToString()
            => _provider.GetQueryText(_expression);
    }
}