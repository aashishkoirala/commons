/*******************************************************************************************************************************
 * AK.Commons.DomainDriven.EntityIdGeneratorProvider
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

using AK.Commons.Composition;
using AK.Commons.Configuration;
using System;
using System.Collections.Generic;

#endregion

namespace AK.Commons.DomainDriven
{

    #region IEntityIdGeneratorProvider

    /// <summary>
    /// Represents something that can give you an IEntityIdGenerator instance for a given key/ID type.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IEntityIdGeneratorProvider
    {
        /// <summary>
        /// Gets an IEntityIdGenerator for the given entity key/ID type.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <returns>IEntityIdGenerator instance.</returns>
        IEntityIdGenerator<TKey> Get<TKey>() where TKey : struct;

        /// <summary>
        /// Gets whether the given key type is supported by this provider.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <returns>Supported?</returns>
        bool SupportsKeyType<TKey>() where TKey : struct;

        /// <summary>
        /// Gets whether the given key type is supported by this provider.
        /// </summary>
        /// <param name="keyType">Key type.</param>
        /// <returns>Supported?</returns>
        bool SupportsKeyType(Type keyType);
    }

    #endregion

    #region EntityIdGeneratorProviderBase

    /// <summary>
    /// Default implementation of IEntityIdGeneratorProvider; it is recommended that all imlementations
    /// inherit from this if possible.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public abstract class EntityIdGeneratorProviderBase : IEntityIdGeneratorProvider, IConfigurableProvider
    {
        #region Constants/Fields

        private const string BatchSizeConfigKey = "BatchSize";

        protected readonly IAppConfig config;
        private readonly IDictionary<Type, object> generatorMap = new Dictionary<Type, object>();
        private readonly object generatorMapLock = new object();
        private bool isConfigAssigned;
        private int batchSize;

        #endregion

        protected EntityIdGeneratorProviderBase(IAppConfig config)
        {
            this.config = config;
        }

        public virtual void AssignConfigKeyPrefix(string configKeyPrefix)
        {
            if (this.isConfigAssigned) return;
            this.isConfigAssigned = true;

            var batchSizeKey = string.Format("{0}.{1}", configKeyPrefix, BatchSizeConfigKey);
            this.batchSize = this.config.Get(batchSizeKey, 100);
        }

        public IEntityIdGenerator<TKey> Get<TKey>() where TKey : struct
        {
            if (!this.SupportsKeyType<TKey>())
            {
                var message = string.Format("Key type {0} not supported.", typeof (TKey).Name);
                throw new NotSupportedException(message);
            }

            var keyType = typeof (TKey);
            object generator;
            if (this.generatorMap.TryGetValue(keyType, out generator))
                return (IEntityIdGenerator<TKey>) generator;

            lock (this.generatorMapLock)
            {
                if (this.generatorMap.TryGetValue(keyType, out generator))
                    return (IEntityIdGenerator<TKey>) generator;

                var newGenerator = this.CreateGenerator<TKey>(this.batchSize);
                this.generatorMap[keyType] = newGenerator;
                return newGenerator;
            }
        }

        /// <summary>
        /// Override this to implement the generator creation logic.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="batchSize">Batch size.</param>
        /// <returns>IEntityIdGenerator instance.</returns>
        protected abstract IEntityIdGenerator<TKey> CreateGenerator<TKey>(int batchSize) where TKey : struct;

        public bool SupportsKeyType<TKey>() where TKey : struct
        {
            return this.SupportsKeyType(typeof (TKey));
        }

        public abstract bool SupportsKeyType(Type keyType);
    }

    #endregion
}