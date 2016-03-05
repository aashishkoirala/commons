using System;

namespace AK.Commons.Caching
{
    public interface ICacheEntry<T>
    {
        bool Exists { get; }
        DateTime CreatedOn { get; }
        DateTime ExpiresOn { get; }
        T Value { get; }
    }
}