using System;

namespace AK.Commons.Threading
{
    public interface ILock : IDisposable
    {
        bool Attained { get; }
        string LockName { get; }
        ILocker Locker { get; }
    }
}