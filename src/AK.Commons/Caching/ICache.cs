using System;

namespace AK.Commons.Caching
{
    public interface ICache
    {
        ICacheEntry<T> Get<T>(string key, Func<T> valueFactory = null);
        ICacheEntry<T> Put<T>(string key, T value);
        void Evict(string key);
    }
}