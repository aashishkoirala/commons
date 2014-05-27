/*******************************************************************************************************************************
 * AK.Commons.DomainDriven.EntityIdGenerator
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
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

using AK.Commons.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace AK.Commons.DomainDriven
{
    #region IEntityIdGenerator

    /// <summary>
    /// Represents something that can give you the next value of an entity key/ID to use.
    /// </summary>
    /// <typeparam name="TKey">Type of entity key/ID.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IEntityIdGenerator<TKey> where TKey : struct
    {
        /// <summary>
        /// Gets the next entity ID value.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Next entity ID value.</returns>
        TKey Next<TEntity>() where TEntity : IEntity<TEntity, TKey>;
    }

    #endregion

    #region EntityIdGeneratorBase

    /// <summary>
    /// Default implementation of IEntityIdGenerator; it is recommended that all implementations
    /// inherit from this if possible.
    /// </summary>
    /// <typeparam name="TKey">Type of entity key/ID.</typeparam>
    /// <author>Aashish Koirala</author>
    public abstract class EntityIdGeneratorBase<TKey> : IEntityIdGenerator<TKey> where TKey : struct
    {
        #region Fields

        private readonly IDictionary<Type, LockedObject<ConcurrentQueue<TKey>>> queueMap =
            new Dictionary<Type, LockedObject<ConcurrentQueue<TKey>>>();

        private readonly object queueMapLock = new object();

        #endregion

        #region Methods (IEntityIdGenerator/Protected/Abstract)

        /// <summary>
        /// Gets or sets the batch size- i.e. the number of entity keys to reserve in one go.
        /// </summary>
        protected int BatchSize { get; set; }

        /// <summary>
        /// Gets the next entity ID value.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Next entity ID value.</returns>
        public TKey Next<TEntity>() where TEntity : IEntity<TEntity, TKey>
        {
            var queue = this.GetQueue<TEntity>();

            while (true)
            {
                var nextKey = GetNextKey(queue);
                if (nextKey.IsThere) return nextKey;

                this.PopulateQueue<TEntity>(queue);
            }
        }

        /// <summary>
        /// Implement this method to provide a way that entity keys are loaded from
        /// whatever the underlying data source is.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="queue">Queue to load keys into.</param>
        protected abstract void PopulateQueue<TEntity>(ConcurrentQueue<TKey> queue);

        #endregion

        #region Methods (Private Helper)

        private LockedObject<ConcurrentQueue<TKey>> GetQueue<TEntity>()
        {
            var entityType = typeof (TEntity);
            LockedObject<ConcurrentQueue<TKey>> queue;

            if (this.queueMap.TryGetValue(entityType, out queue)) return queue;

            lock (this.queueMapLock)
            {
                if (this.queueMap.TryGetValue(entityType, out queue)) return queue;

                queue = new LockedObject<ConcurrentQueue<TKey>>(new ConcurrentQueue<TKey>());
                this.queueMap[entityType] = queue;
                return queue;
            }
        }

        private static Perhaps<TKey> GetNextKey(LockedObject<ConcurrentQueue<TKey>> queue)
        {
            var nextKey = Perhaps<TKey>.NotThere;
            queue.ExecuteRead(x =>
                {
                    TKey key;
                    if (x.TryDequeue(out key)) nextKey = key;
                });
            return nextKey;
        }

        private void PopulateQueue<TEntity>(LockedObject<ConcurrentQueue<TKey>> queue)
        {
            queue.ExecuteWrite(x =>
                {
                    if (!x.IsEmpty) return;
                    this.PopulateQueue<TEntity>(x);
                });
        }

        #endregion
    }

    #endregion
}