using System;

namespace AK.Commons.Threading
{
    public interface ILocker
    {
        ILock Lock(string lockName, int attempts = 1, TimeSpan? interAttemptDelay = null);
    }
}