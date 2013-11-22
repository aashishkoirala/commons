/*******************************************************************************************************************************
 * AK.Commons.Providers.Composition.Composer
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
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
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Providers.Composition
{
    /// <summary>
    /// The one and only internal implementation of IComposer.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class Composer : IComposer
    {
        private const string ModulesDirectoriesConfigKey = "ak.commons.composition.modulesdirectories";

        public CompositionContainer Container { get; private set; }

        public Composer(IAppConfig config, IAppLogger logger)
        {
            var modulesDirectories = config.Get(ModulesDirectoriesConfigKey, string.Empty)
                .Split(new[] {';'})
                .Select(x => x.ToLower())
                .ToList();

            var entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var entryAssemblyDirectory = (Path.GetDirectoryName(entryAssembly.Location) ?? string.Empty).ToLower();

            if (!string.IsNullOrWhiteSpace(entryAssemblyDirectory) && !modulesDirectories.Contains(entryAssemblyDirectory))
                modulesDirectories.Add(entryAssemblyDirectory);

            var assemblyFiles = new List<string>();

            foreach (var modulesDirectory in modulesDirectories)
            {
                var dir = modulesDirectory;
                ManageWebRelativePath(ref dir);
                ManageRelativePath(entryAssemblyDirectory, ref dir);

                using (var directoryCatalog = new DirectoryCatalog(dir))
                    assemblyFiles.AddRange(directoryCatalog.LoadedFiles);
            }

            var assemblyCatalogs = assemblyFiles
                .Distinct()
                .Select(Assembly.LoadFrom)
                .Distinct(new AssemblyEqualityComparer())
                .Select(x => new AssemblyCatalog(x))
                .ToList();

            this.Container = new CompositionContainer(new AggregateCatalog(assemblyCatalogs), true);
        }

        #region Methods (Resolve*)

        public TContract Resolve<TContract>(string contractName = null)
        {
            var exportedValues = this.ResolveMany<TContract>(contractName);

            return exportedValues.Single();
        }

        public TContract Resolve<TContract, TMetadata>(string contractName = null)
        {
            var exportedValues = this.ResolveMany<TContract, TMetadata>(contractName);

            return exportedValues.Single();
        }

        public TContract Resolve<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null)
        {
            var exportedValues = this.ResolveMany<TContract, TMetadata>(metadataFilterFunc, contractName);

            return exportedValues.Single();
        }

        public Lazy<TContract> ResolveLazy<TContract>(string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract>(contractName);

            return exports.Single();
        }

        public Lazy<TContract> ResolveLazy<TContract, TMetadata>(string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract, TMetadata>(contractName);

            return exports.Single();
        }

        public Lazy<TContract> ResolveLazy<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract, TMetadata>(metadataFilterFunc, contractName);

            return exports.Single();
        }

        public IEnumerable<TContract> ResolveMany<TContract>(string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract>(contractName);

            return exports.Select(x => x.Value);
        }

        public IEnumerable<TContract> ResolveMany<TContract, TMetadata>(string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract, TMetadata>(contractName);
            
            return exports.Select(x => x.Value);
        }

        public IEnumerable<TContract> ResolveMany<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract, TMetadata>(metadataFilterFunc, contractName);

            return exports.Select(x => x.Value);
        }

        public IEnumerable<Lazy<TContract>> ResolveManyLazy<TContract>(string contractName = null)
        {
            return contractName == null
                       ? this.Container.GetExports<TContract>()
                       : this.Container.GetExports<TContract>(contractName);
        }

        public IEnumerable<Lazy<TContract, TMetadata>> ResolveManyLazy<TContract, TMetadata>(string contractName = null)
        {
            return contractName == null
                       ? this.Container.GetExports<TContract, TMetadata>()
                       : this.Container.GetExports<TContract, TMetadata>(contractName);
        }

        public IEnumerable<Lazy<TContract>> ResolveManyLazy<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null)
        {
            var exports = this.ResolveManyLazy<TContract, TMetadata>(contractName);

            if (metadataFilterFunc != null)
                exports = exports.Where(x => metadataFilterFunc(x.Metadata));

            return exports;
        }

        public object Resolve(Type contractType, string contractName = null)
        {
            var exportedValues = this.ResolveMany(contractType, null, contractName);

            return exportedValues.Single();
        }

        public object Resolve(Type contractType, Type metadataType, string contractName = null)
        {
            var exportedValues = this.ResolveMany(contractType, metadataType, contractName);

            return exportedValues.Single();
        }

        public IEnumerable<object> ResolveMany(Type contractType, Type metadataType = null, string contractName = null)
        {
            return this.ResolveManyLazy(contractType, metadataType, contractName).Select(x => x.Value);
        }

        public IEnumerable<Lazy<object, object>> ResolveManyLazy(Type contractType, Type metadataType = null, string contractName = null)
        {
            return this.Container.GetExports(contractType, metadataType, contractName);
        }

        #endregion

        #region Methods (Private) & Nested Types

        private static void ManageWebRelativePath(ref string modulesDirectory)
        {
            if (!modulesDirectory.StartsWith("~/")) return;
            if (HttpContext.Current == null) return;

            modulesDirectory = HttpContext.Current.Server.MapPath(modulesDirectory);
        }

        private static void ManageRelativePath(string entryAssemblyDirectory, ref string modulesDirectory)
        {
            if (Path.IsPathRooted(modulesDirectory)) return;

            modulesDirectory = Path.GetFullPath(Path.Combine(entryAssemblyDirectory, modulesDirectory));
        }

        private class AssemblyEqualityComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                return x.FullName == y.FullName;
            }

            public int GetHashCode(Assembly obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
    }
}