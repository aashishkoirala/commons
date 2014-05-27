/*******************************************************************************************************************************
 * AK.Commons.Providers.DomainDriven.DatabaseEntityIdGenerator
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

using AK.Commons.DataAccess;
using AK.Commons.DomainDriven;
using System.Collections.Concurrent;

#endregion

namespace AK.Commons.Providers.DomainDriven
{
    /// <summary>
    /// Implementation of IEntityIdGenerator that uses a database table as the storage mechanism.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class DatabaseEntityIdGenerator<TKey> : EntityIdGeneratorBase<TKey> where TKey : struct
    {
        private readonly IUnitOfWorkFactory database;
        private readonly string collectionName;
        private readonly int batchSize;

        public DatabaseEntityIdGenerator(IUnitOfWorkFactory database, string collectionName, int batchSize)
        {
            this.database = database;
            this.collectionName = collectionName;
            this.batchSize = batchSize;
        }

        protected override void PopulateQueue<TEntity>(ConcurrentQueue<TKey> queue)
        {
            var sequenceName = typeof (TEntity).FullName;

            if (typeof (TKey) == typeof (short))
                this.PopulateQueue(sequenceName, (ConcurrentQueue<short>) (object) queue);

            if (typeof (TKey) == typeof (int))
                this.PopulateQueue(sequenceName, (ConcurrentQueue<int>) (object) queue);

            if (typeof (TKey) == typeof (long))
                this.PopulateQueue(sequenceName, (ConcurrentQueue<long>) (object) queue);
        }

        private void PopulateQueue(string sequenceName, ConcurrentQueue<short> queue)
        {
            short lastKey;
            using (var unit = this.database.Create())
            {
                lastKey = unit.NextValueInSequence(
                    this.collectionName, sequenceName, (short) this.batchSize);
            }

            var nextLastKey = lastKey + (short) this.batchSize;

            for (var key = (short) (lastKey + 1); key <= nextLastKey; key++)
                queue.Enqueue(key);
        }

        private void PopulateQueue(string sequenceName, ConcurrentQueue<int> queue)
        {
            int lastKey;
            using (var unit = this.database.Create())
            {
                lastKey = unit.NextValueInSequence(
                    this.collectionName, sequenceName, (short) this.batchSize);
            }

            var nextLastKey = lastKey + (short) this.batchSize;

            for (var key = (short) (lastKey + 1); key <= nextLastKey; key++)
                queue.Enqueue(key);
        }

        private void PopulateQueue(string sequenceName, ConcurrentQueue<long> queue)
        {
            long lastKey;
            using (var unit = this.database.Create())
            {
                lastKey = unit.NextValueInSequence(
                    this.collectionName, sequenceName, (short) this.batchSize);
            }

            var nextLastKey = lastKey + (short) this.batchSize;

            for (var key = (short) (lastKey + 1); key <= nextLastKey; key++)
                queue.Enqueue(key);
        }
    }
}