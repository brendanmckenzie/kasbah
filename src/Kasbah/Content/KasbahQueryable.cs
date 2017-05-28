using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kasbah.Content
{
    public class KasbahQueryable<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        readonly IQueryProvider _provider;
        readonly Expression _expression;

        public KasbahQueryable(IQueryProvider provider)
        {
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public KasbahQueryable(IQueryProvider provider, Expression expression)
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
    }
}
