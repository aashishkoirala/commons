/*******************************************************************************************************************************
 * AK.Commons.Providers.DomainDriven.DatabaseEntityIdGeneratorProvider
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
using AK.Commons.DataAccess;
using AK.Commons.DomainDriven;
using System;
using System.ComponentModel.Composition;
using System.Linq;

#endregion

namespace AK.Commons.Providers.DomainDriven
{
    /// <summary>
    /// Implementation of IEntityIdGeneratorProvider that provides instances of DatabaseEntityIdGenerator.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IEntityIdGeneratorProvider)),
     PartCreationPolicy(CreationPolicy.Shared),
     ProviderMetadata("Database")]
    public class DatabaseEntityIdGeneratorProvider : EntityIdGeneratorProviderBase
    {
        private const string DataAccessConfigKey = "DataAccess";
        private const string CollectionNameConfigKey = "CollectionName";

        private readonly IAppDataAccess dataAccess;
        private IUnitOfWorkFactory database;
        private string collectionName;

        [ImportingConstructor]
        public DatabaseEntityIdGeneratorProvider(
            [Import] IAppConfig config,
            [Import] IAppDataAccess dataAccess) : base(config)
        {
            this.dataAccess = dataAccess;
        }

        public override void AssignConfigKeyPrefix(string configKeyPrefix)
        {
            base.AssignConfigKeyPrefix(configKeyPrefix);

            var dataAccessKey = string.Format("{0}.{1}", configKeyPrefix, DataAccessConfigKey);
            var collectionNameKey = string.Format("{0}.{1}", configKeyPrefix, CollectionNameConfigKey);

            var dataAccessName = this.config.Get<string>(dataAccessKey);
            this.database = this.dataAccess[dataAccessName];
            this.collectionName = this.config.Get<string>(collectionNameKey);
        }

        protected override IEntityIdGenerator<TKey> CreateGenerator<TKey>(int batchSize)
        {
            return new DatabaseEntityIdGenerator<TKey>(this.database, this.collectionName, batchSize);
        }

        public override bool SupportsKeyType(Type keyType)
        {
            return new[] {typeof (short), typeof (int), typeof (long)}.Any(x => x == keyType);
        }
    }
}