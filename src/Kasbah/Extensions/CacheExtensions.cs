using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Kasbah.Extensions
{
    public static class CacheExtensions
    {
        public static async Task<T> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> generator)
        {
            if (cache == null)
            {
                return await generator();
            }

            // TODO: find out why GetStringAsync/SetStringAsync don't work
            var cacheValue = cache.GetString(key);
            if (cacheValue == null)
            {
                var generatedValue = await generator();

                cache.SetString(key, JsonConvert.SerializeObject(generatedValue));

                return generatedValue;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(cacheValue);
            }
        }
    }
}