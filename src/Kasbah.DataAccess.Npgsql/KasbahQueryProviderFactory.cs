using System;
using System.Linq;
using Kasbah.Content;

namespace Kasbah.DataAccess.Npgsql
{
    public class KasbahQueryProviderFactory : IKasbahQueryProviderFactory
    {
        readonly NpgsqlSettings _settings;
        readonly TypeRegistry _typeRegistry;
        readonly TypeMapper _typeMapper;

        public KasbahQueryProviderFactory(NpgsqlSettings settings, TypeRegistry typeRegistry, TypeMapper typeMapper)
        {
            _settings = settings;
            _typeRegistry = typeRegistry;
            _typeMapper = typeMapper;
        }

        public IQueryProvider CreateProvider(Type targetType)
        {
            return new KasbahNpgsqlQueryProvider(targetType, _settings, _typeRegistry, _typeMapper);
        }
    }
}
