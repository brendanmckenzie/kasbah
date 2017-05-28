using System;
using System.Linq;

namespace Kasbah.Content
{
    public interface IKasbahQueryProviderFactory
    {
        IQueryProvider CreateProvider(Type targetType);
    }
}
