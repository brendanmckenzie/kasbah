using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasbah.Content
{
    public class TypeMapperContext
    {
        readonly IDictionary<string, object> _cache;

        public TypeMapperContext()
        {
            _cache = new Dictionary<string, object>();
        }

        public async Task<object> GetOrSetAsync(string key, Func<Task<object>> generator)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, await generator());
            }

            return _cache[key];
        }
    }
}
