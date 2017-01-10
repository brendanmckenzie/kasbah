using System.Collections.Generic;

namespace Kasbah
{
    public static class DictionaryExtensions
    {
        public static TVal SafeGet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            return default(TVal);
        }
    }
}