using System;
using System.Linq;
using Kasbah.Content;
using Microsoft.Extensions.Logging;

namespace Kasbah.DataAccess.Npgsql
{
    public class KasbahQueryProviderFactory : IKasbahQueryProviderFactory
    {
        readonly ILoggerFactory _loggerFactory;
        readonly NpgsqlSettings _settings;
        readonly TypeRegistry _typeRegistry;
        readonly TypeMapper _typeMapper;

        public KasbahQueryProviderFactory(ILoggerFactory loggerFactory, NpgsqlSettings settings, TypeRegistry typeRegistry, TypeMapper typeMapper)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _typeRegistry = typeRegistry;
            _typeMapper = typeMapper;
        }

        public IQueryProvider CreateProvider(Type targetType)
            => new KasbahNpgsqlQueryProvider(_loggerFactory, targetType, _settings, _typeRegistry, _typeMapper);
    }
}
