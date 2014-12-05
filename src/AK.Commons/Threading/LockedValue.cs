/*******************************************************************************************************************************
 * AK.Commons.Threading.LockedValue
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

using System.Threading;

namespace AK.Commons.Threading
{
    /// <summary>
    /// Wraps a value in a reader/writer protected context. Use this only for primitive types and pure value types,
    /// i.e. "struct"s that do not have any reference type members.
    /// </summary>
    /// <author>Aashish Koirala</author>
    /// <typeparam name="T">Type of value.</typeparam>
    public class LockedValue<T> : LockedObject<T> where T : struct
    {
        /// <summary>
        /// Creates a new reader/writer protection wrapped value.
        /// </summary>
        /// <param name="value">Object to wrap.</param>
        public LockedValue(T value) : base(value) { }

        /// <summary>
        /// Creates a new reader/writer protection wrapped value.
        /// </summary>
        /// <param name="value">Object to wrap.</param>
        /// <param name="recursionPolicy">Lock recursion policy to use.</param>
        public LockedValue(T value, LockRecursionPolicy recursionPolicy) : base(value, recursionPolicy) { }

        /// <summary>
        /// Gets or sets the value of the protected resource. Gets the value within a read lock, and
        /// sets the value within a write lock. Waits indefinitely to acquire the lock in both cases.
        /// </summary>
        public T Value
        {
            get { return this.ExecuteRead(v => v); }
            set { this.Update(value); }
        }
    }
}