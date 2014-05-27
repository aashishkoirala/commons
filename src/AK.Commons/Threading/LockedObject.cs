/*******************************************************************************************************************************
 * AK.Commons.Threading.LockedObject
 * Copyright © 2013-2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library (AKCL).
 *  
 * AKCL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.Threading;

#endregion

namespace AK.Commons.Threading
{
    /// <summary>
    /// Wraps an object in a reader/writer protected context.
    /// </summary>
    /// <author>Aashish Koirala</author>
    /// <typeparam name="T">Type of object.</typeparam>
    public class LockedObject<T> : IDisposable
    {
        #region Fields

        private readonly ReaderWriterLockSlim readerWriterLockSlim;
        private T value;

        #endregion

        #region Constructors/IDisposable

        /// <summary>
        /// Creates a new reader/writer protection wrapped instance.
        /// </summary>
        /// <param name="value">Object to wrap.</param>
        public LockedObject(T value)
        {
            this.value = value;
            this.readerWriterLockSlim = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Creates a new reader/writer protection wrapped instance.
        /// </summary>
        /// <param name="value">Object to wrap.</param>
        /// <param name="recursionPolicy">Lock recursion policy to use.</param>
        public LockedObject(T value, LockRecursionPolicy recursionPolicy)
        {
            this.value = value;
            this.readerWriterLockSlim = new ReaderWriterLockSlim(recursionPolicy);
        }

        ~LockedObject()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) this.readerWriterLockSlim.Dispose();
        }

        #endregion

        #region Methods - Public (Read)

        /// <summary>
        /// Executes the given action on the protected resource within a read lock.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="timeout">
        /// Time to wait for the read lock before giving up. If a read lock can't be acquired
        /// within the given time, a TimeoutException is thrown. If omitted or NULL is passed,
        /// there is no timeout and we wait indefinitely.
        /// </param>
        public void ExecuteRead(Action<T> action, TimeSpan? timeout = null)
        {
            this.EnterReadLock(timeout);

            try
            {
                action(this.value);
            }
            finally
            {
                this.readerWriterLockSlim.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes the given action on the protected resource within a read lock that returns
        /// a value.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="timeout">
        /// Time to wait for the read lock before giving up. If a read lock can't be acquired
        /// within the given time, a TimeoutException is thrown. If omitted or NULL is passed,
        /// there is no timeout and we wait indefinitely.
        /// </param>
        /// <returns>Value from the action performed.</returns>
        public TResult ExecuteRead<TResult>(Func<T, TResult> action, TimeSpan? timeout = null)
        {
            this.EnterReadLock(timeout);

            try
            {
                return action(this.value);
            }
            finally
            {
                this.readerWriterLockSlim.ExitReadLock();
            }
        }

        #endregion

        #region Methods - Public (Write)

        /// <summary>
        /// Executes the given action on the protected resource within a write lock.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="timeout">
        /// Time to wait for the write lock before giving up. If a write lock can't be acquired
        /// within the given time, a TimeoutException is thrown. If omitted or NULL is passed,
        /// there is no timeout and we wait indefinitely.
        /// </param>
        public void ExecuteWrite(Action<T> action, TimeSpan? timeout = null)
        {
            this.EnterWriteLock(timeout);

            try
            {
                action(this.value);
            }
            finally
            {
                this.readerWriterLockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Executes the given action on the protected resource within a write lock that returns
        /// a value.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="timeout">
        /// Time to wait for the write lock before giving up. If a write lock can't be acquired
        /// within the given time, a TimeoutException is thrown. If omitted or NULL is passed,
        /// there is no timeout and we wait indefinitely.
        /// </param>
        /// <returns>Value from the action performed.</returns>
        public TResult ExecuteWrite<TResult>(Func<T, TResult> action, TimeSpan? timeout = null)
        {
            this.EnterWriteLock(timeout);

            try
            {
                return action(this.value);
            }
            finally
            {
                this.readerWriterLockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Replaces the protected resource with a new one. Does this within a write lock.
        /// </summary>
        /// <param name="newValue">New resource to use.</param>
        /// <param name="timeout">
        /// Time to wait for the write lock before giving up. If a write lock can't be acquired
        /// within the given time, a TimeoutException is thrown. If omitted or NULL is passed,
        /// there is no timeout and we wait indefinitely.
        /// </param>
        public void Update(T newValue, TimeSpan? timeout = null)
        {
            this.ExecuteWrite(v => this.value = newValue, timeout);
        }

        #endregion

        #region Methods - Private

        private void EnterReadLock(TimeSpan? timeout)
        {
            if (!timeout.HasValue) this.readerWriterLockSlim.EnterReadLock();
            else if (!this.readerWriterLockSlim.TryEnterReadLock(timeout.Value)) throw new TimeoutException();            
        }

        private void EnterWriteLock(TimeSpan? timeout)
        {
            if (!timeout.HasValue) this.readerWriterLockSlim.EnterWriteLock();
            else if (!this.readerWriterLockSlim.TryEnterWriteLock(timeout.Value)) throw new TimeoutException();
        }

        #endregion
    }
}