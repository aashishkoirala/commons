/*******************************************************************************************************************************
 * AK.Commons.Providers.DomainDriven.FileSystemEntityIdGeneratorProvider
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
using AK.Commons.DomainDriven;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace AK.Commons.Providers.DomainDriven
{
    /// <summary>
    /// Implementation of IEntityIdGeneratorProvider that provides FileSystemEntityIdGenerator instances.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IEntityIdGeneratorProvider)),
     PartCreationPolicy(CreationPolicy.Shared),
     ProviderMetadata("FileSystem")]
    public class FileSystemEntityIdGeneratorProvider : EntityIdGeneratorProviderBase
    {
        private const string FolderConfigKey = "Folder";

        private string folder;

        [ImportingConstructor]
        public FileSystemEntityIdGeneratorProvider([Import] IAppConfig config) : base(config)
        {
        }

        public override void AssignConfigKeyPrefix(string configKeyPrefix)
        {
            base.AssignConfigKeyPrefix(configKeyPrefix);

            var folderKey = string.Format("{0}.{1}", configKeyPrefix, FolderConfigKey);
            this.folder = this.config.Get<string>(folderKey);

            if (Path.IsPathRooted(this.folder)) return;

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(dir != null);

            this.folder = Path.Combine(dir, this.folder);
        }

        protected override IEntityIdGenerator<TKey> CreateGenerator<TKey>(int batchSize)
        {
            return new FileSystemEntityIdGenerator<TKey>(this.folder, batchSize);
        }

        public override bool SupportsKeyType(Type keyType)
        {
            return new[] {typeof (short), typeof (int), typeof (long)}.Any(x => x == keyType);
        }
    }
}