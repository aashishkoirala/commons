using AK.Commons.Composition;
using AK.Commons.Threading;
using System;
using System.ComponentModel.Composition;
using System.Security.AccessControl;
using System.Threading;

namespace AK.Commons.Providers.Threading
{
    [Export(typeof (ILocker)), PartCreationPolicy(CreationPolicy.Shared), ProviderMetadata("InMemory")]
    public class InMemoryLocker : ILocker, IConfigurableProvider
    {
        public void AssignConfigKeyPrefix(string configKeyPrefix)
        {
        }

        public ILock Lock(string lockName, int attempts = 1, TimeSpan? interAttemptDelay = null)
        {
            lockName = $"AK.Commons.InMemoryLocker.{lockName}";

            Semaphore semaphore;
            if (!Semaphore.TryOpenExisting(lockName, SemaphoreRights.FullControl, out semaphore))
                semaphore = new Semaphore(1, 1, lockName);

            var result = new InMemoryLock {Attained = false, LockName = lockName, Locker = this};

            var attempt = attempts;
            while (attempt > 0)
            {
                result.Attained = semaphore.WaitOne(TimeSpan.Zero);
                if (result.Attained)
                {
                    result.Semaphore = semaphore;
                    break;
                }

                var sleep = interAttemptDelay ?? TimeSpan.FromMinutes(1);
                Thread.Sleep(sleep);
                attempt--;
            }

            return result;
        }

        private class InMemoryLock : ILock
        {
            public void Dispose()
            {
                if (!this.Attained) return;

                this.Semaphore.Release();
                this.Semaphore.Dispose();
                this.Semaphore = null;
            }

            public bool Attained { get; set; }
            public string LockName { get; set; }
            public ILocker Locker { get; set; }
            public Semaphore Semaphore { private get; set; }
        }
    }
}