/*******************************************************************************************************************************
 * AK.Commons.Providers.DomainDriven.FileSystemEntityIdGenerator
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

using AK.Commons.DomainDriven;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading;

#endregion

namespace AK.Commons.Providers.DomainDriven
{
    /// <summary>
    /// Implementation of IEntityIdGenerator that uses the file system as the storage mechanism.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class FileSystemEntityIdGenerator<TKey> : EntityIdGeneratorBase<TKey> where TKey : struct
    {
        private readonly string folder;
        private readonly int batchSize;

        public FileSystemEntityIdGenerator(string folder, int batchSize)
        {
            this.folder = folder;
            this.batchSize = batchSize;
        }

        protected override void PopulateQueue<TEntity>(ConcurrentQueue<TKey> queue)
        {
            var fileName = string.Format("{0}_{1}.key", typeof(TEntity).FullName, typeof(TKey).FullName);
            fileName = Path.Combine(this.folder, fileName);

            var semaphoreName = string.Format("AK.Commons.DomainDriven.EntityIdGenerator.FileSystem.{0}",
                                              fileName.Replace("\\", "_"));

            using (var semaphore = OpenOrCreateSemaphore(semaphoreName))
            {
                semaphore.WaitOne();
                this.PopulateQueue(fileName, queue);
                semaphore.Release();
            }
        }

        private void PopulateQueue(string fileName, ConcurrentQueue<TKey> queue)
        {
            var content = File.Exists(fileName) ? File.ReadAllText(fileName) : "0";

            if (typeof (TKey) == typeof (short))
                content = this.PopulateQueueAndGetNextLastKeyAsString(content, (ConcurrentQueue<short>) (object) queue);

            if (typeof(TKey) == typeof(int))
                content = this.PopulateQueueAndGetNextLastKeyAsString(content, (ConcurrentQueue<int>)(object)queue);

            if (typeof(TKey) == typeof(long))
                content = this.PopulateQueueAndGetNextLastKeyAsString(content, (ConcurrentQueue<long>)(object)queue);

            File.WriteAllText(fileName, content);
        }

        private string PopulateQueueAndGetNextLastKeyAsString(string lastKeyAsString, ConcurrentQueue<short> queue)
        {
            short lastKey;
            if (!short.TryParse(lastKeyAsString, out lastKey))
                lastKey = 0;

            var nextLastKey = lastKey + (short)this.batchSize;

            for (var key = (short)(lastKey + 1); key <= nextLastKey; key++)
                queue.Enqueue(key);

            return nextLastKey.ToString(CultureInfo.InvariantCulture);
        }

        private string PopulateQueueAndGetNextLastKeyAsString(string lastKeyAsString, ConcurrentQueue<int> queue)
        {
            int lastKey;
            if (!int.TryParse(lastKeyAsString, out lastKey))
                lastKey = 0;

            var nextLastKey = lastKey + this.batchSize;

            for (var key = lastKey + 1; key <= nextLastKey; key++)
                queue.Enqueue(key);

            return nextLastKey.ToString(CultureInfo.InvariantCulture);
        }

        private string PopulateQueueAndGetNextLastKeyAsString(string lastKeyAsString, ConcurrentQueue<long> queue)
        {
            long lastKey;
            if (!long.TryParse(lastKeyAsString, out lastKey))
                lastKey = 0;

            var nextLastKey = lastKey + this.batchSize;

            for (var key = lastKey + 1; key <= nextLastKey; key++)
                queue.Enqueue(key);

            return nextLastKey.ToString(CultureInfo.InvariantCulture);
        }

        private static Semaphore OpenOrCreateSemaphore(string name)
        {
            Semaphore result;
            var exists = Semaphore.TryOpenExisting(name, out result);

            if (exists) return result;

            result = new Semaphore(1, 1, name);
            return result;
        }
    }
}