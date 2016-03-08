using AK.Commons.Caching;
using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace AK.Commons.Providers.Caching
{
    [Export(typeof (ICache)), PartCreationPolicy(CreationPolicy.NonShared), ProviderMetadata("InMemory")]
    public class InMemoryCache : ICache, IConfigurableProvider
    {
        private const string CacheNameConfigKey = "CacheName";
        private const string ExpiryMillisecondsConfigKey = "ExpiryMilliseconds";

        private static readonly LockedObject<IDictionary<string, LockedObject<IDictionary<string, object>>>> CacheHash =
            new LockedObject<IDictionary<string, LockedObject<IDictionary<string, object>>>>(
                new Dictionary<string, LockedObject<IDictionary<string, object>>>());

        private readonly IAppConfig config;

        private bool isConfigured;
        private string cacheName;
        private int expiryMilliseconds;
        private LockedObject<IDictionary<string, object>> cache;

        [ImportingConstructor]
        public InMemoryCache([Import] IAppConfig config)
        {
            this.config = config;
        }

        public void AssignConfigKeyPrefix(string configKeyPrefix)
        {
            if (this.isConfigured) return;
            this.isConfigured = true;

            var cacheNameKey = $"{configKeyPrefix}.{CacheNameConfigKey}";
            this.cacheName = this.config.Get<string>(cacheNameKey);

            var expiryMillisecondsKey = $"{configKeyPrefix}.{ExpiryMillisecondsConfigKey}";
            this.expiryMilliseconds = this.config.Get(expiryMillisecondsKey, (int) TimeSpan.FromMinutes(10).TotalMilliseconds);

            this.cache = CacheHash.ExecuteRead(x => x.LookFor(this.cacheName)).ValueOrDefault;
            if (this.cache != null) return;

            this.cache = new LockedObject<IDictionary<string, object>>(new Dictionary<string, object>());
            CacheHash.ExecuteWrite(x => x[this.cacheName] = this.cache);
        }

        public ICacheEntry<T> Get<T>(string key, Func<T> valueFactory = null)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var perhapsEntry = this.cache.ExecuteRead(x => x.LookFor(key));
            if (perhapsEntry.IsThere)
            {
                var entry = (ICacheEntry<T>) perhapsEntry.Value;
                if (entry.ExpiresOn > DateTime.Now) return entry;
            }
            if (valueFactory == null) return new InMemoryCacheEntry<T>(false, DateTime.Now, DateTime.Now, default(T));
            var newEntry = new InMemoryCacheEntry<T>(true, DateTime.Now,
                DateTime.Now.AddMilliseconds(this.expiryMilliseconds), valueFactory());
            this.Put(key, newEntry);
            return newEntry;
        }

        public ICacheEntry<T> Put<T>(string key, T value)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var entry = new InMemoryCacheEntry<T>(true, DateTime.Now,
                DateTime.Now.AddMilliseconds(this.expiryMilliseconds), value);
            this.cache.ExecuteWrite(x => x[key] = entry);
            return entry;
        }

        public void Evict(string key)
        {
            if (!this.isConfigured) throw new InvalidOperationException("Not configured.");

            var exists = this.cache.ExecuteRead(x => x.ContainsKey(key));
            if (!exists) return;
            this.cache.ExecuteWrite(x => x.Remove(key));
        }

        private class InMemoryCacheEntry<T> : ICacheEntry<T>
        {
            public InMemoryCacheEntry(bool exists, DateTime createdOn, DateTime expiresOn, T value)
            {
                this.Exists = exists;
                this.CreatedOn = createdOn;
                this.ExpiresOn = expiresOn;
                this.Value = value;
            }

            public bool Exists { get; }
            public DateTime CreatedOn { get; }
            public DateTime ExpiresOn { get; }
            public T Value { get; }
        }
    }
}