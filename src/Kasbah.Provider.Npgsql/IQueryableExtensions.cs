using System.Linq;
using System.Linq.Expressions;

namespace Kasbah.Provider.Npgsql
{
    internal static class IQueryableExtensions
    {
        public static string GetQueryText(this IQueryProvider provider, Expression expression)
        {
            return expression.ToString();
        }
    }
}
