using System;
using System.Collections.Generic;

namespace AK.Commons.Caching
{
    public interface ICache
    {
        IDictionary<string, T> AsDictionary<T>();
        ICacheEntry<T> Get<T>(string key, Func<T> valueFactory = null);
        ICacheEntry<T> Put<T>(string key, T value);
        void Evict(string key);
    }
}