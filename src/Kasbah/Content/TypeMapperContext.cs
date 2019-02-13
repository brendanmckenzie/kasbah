using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasbah.Content
{
    public class TypeMapperContext
    {
        readonly ConcurrentDictionary<string, object> _cache;

        public TypeMapperContext()
        {
            _cache = new ConcurrentDictionary<string, object>();
        }

        public async Task<object> GetOrSetAsync(string key, Func<Task<object>> generator)
        {
            // TODO: use GetOrAdd()
            if (!_cache.ContainsKey(key))
            {
                _cache.TryAdd(key, await generator());
            }

            return _cache[key];
        }
    }
}
