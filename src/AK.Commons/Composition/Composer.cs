/*******************************************************************************************************************************
 * AK.Commons.Composition.Composer
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

using AK.Commons.Configuration;
using AK.Commons.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

#endregion

namespace AK.Commons.Composition
{
    #region IComposer

    /// <summary>
    /// Interface that provides composition services. You should not implement this interface directly,
    /// but rather use the one system implementation of this. To do that:
    /// 
    /// 1) Make sure you've called AppEnvironment.Initialize().
    /// 2) Either:
    ///    a) Create a MEF import against IComposer (preferably), or:
    ///    b) Use AppEnvironment.Composer.
    /// 
    /// The system implementation of this is basically just a glorified MEF wrapper.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IComposer
    {
        /// <summary>
        /// Lets you directly access the underlying MEF container, if you want to directly get at it.
        /// </summary>
        CompositionContainer Container { get; }

        /// <summary>
        /// Resolves the given contract to its one implementation.
        /// Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        TContract Resolve<TContract>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation with a metadata attribute applied that implements
        /// the given metadata contract. Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        TContract Resolve<TContract, TMetadata>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation with a metadata attribute applied that implements
        /// the given metadata contract. Filters the list down to those exports whose metadata satisfy a condition
        /// provided as one of the parameters. Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="metadataFilterFunc">Condition that the metadata is supposed to satisfy.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Resolved instance of the contract.</returns>
        TContract Resolve<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation. Does not instantiate the dependency,
        /// just returns a lazy pointer to it that you can instantiate at your will.
        /// Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Lazy pointer to resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        Lazy<TContract> ResolveLazy<TContract>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation with a metadata attribute applied that implements
        /// the given metadata contract. Does not instantiate the dependency, just returns a lazy pointer to it
        /// that you can instantiate at your will. Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Lazy pointer to resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        Lazy<TContract> ResolveLazy<TContract, TMetadata>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation with a metadata attribute applied that implements
        /// the given metadata contract. Filters the list down to those exports whose metadata satisfy a condition
        /// provided as one of the parameters. Does not instantiate the dependency, just returns a lazy pointer to it
        /// that you can instantiate at your will. Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <param name="metadataFilterFunc">Condition that the metadata is supposed to satisfy.</param>
        /// <returns>Lazy pointer to resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        Lazy<TContract> ResolveLazy<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of resolved instances of the contract.</returns>
        IEnumerable<TContract> ResolveMany<TContract>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations with a metadata attribute applied that implements
        /// the given metadata contract.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of resolved instances of the contract.</returns>
        IEnumerable<TContract> ResolveMany<TContract, TMetadata>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations with a metadata attribute applied that implements
        /// the given metadata contract. Filters the list down to those exports whose metadata satisfy a condition
        /// provided as one of the parameters.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="metadataFilterFunc">Condition that the metadata is supposed to satisfy.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of resolved instances of the contract.</returns>
        IEnumerable<TContract> ResolveMany<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations. Does not instantiate the dependency,
        /// just returns lazy pointers to it that you can instantiate at your will.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of lazy pointers to resolved instances of the contract.</returns>
        IEnumerable<Lazy<TContract>> ResolveManyLazy<TContract>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations with a metadata attribute applied that implements
        /// the given metadata contract. Does not instantiate the dependency, just returns lazy pointers to it
        /// that you can instantiate at your will.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of lazy pointers to resolved instances of the contract.</returns>
        IEnumerable<Lazy<TContract, TMetadata>> ResolveManyLazy<TContract, TMetadata>(string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations with a metadata attribute applied that implements
        /// the given metadata contract. Filters the list down to those exports whose metadata satisfy a condition
        /// provided as one of the parameters. Does not instantiate the dependency, just returns lazy pointers to it
        /// that you can instantiate at your will.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TMetadata">Metadata contract type.</typeparam>
        /// <param name="metadataFilterFunc">Condition that the metadata is supposed to satisfy.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of lazy pointers to resolved instances of the contract.</returns>
        IEnumerable<Lazy<TContract>> ResolveManyLazy<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc, string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation.
        /// Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        object Resolve(Type contractType, string contractName = null);

        /// <summary>
        /// Resolves the given contract to its one implementation with a metadata attribute applied that implements
        /// the given metadata contract. Fails if there are more than one. Fails if there are none.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="metadataType">Metadata contract type.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>Resolved instance of the contract.</returns>
        /// <exception cref="ComposerException">If no exports are found, or if too many exports are found.</exception>
        object Resolve(Type contractType, Type metadataType, string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations with a metadata attribute applied that implements
        /// the given metadata contract. 
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="metadataType">Metadata contract type.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of resolved instances of the contract.</returns>
        IEnumerable<object> ResolveMany(Type contractType, Type metadataType = null, string contractName = null);

        /// <summary>
        /// Resolves the given contract to all its implementations with a metadata attribute applied that implements
        /// the given metadata contract. Does not instantiate the dependency, just returns lazy pointers to it
        /// that you can instantiate at your will.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="metadataType">Metadata contract type.</param>
        /// <param name="contractName">
        /// Contract name (optional; if not provided it will look for exports that do not provide a contract name).
        /// </param>
        /// <returns>List of lazy pointers to resolved instances of the contract.</returns>
        IEnumerable<Lazy<object, object>> ResolveManyLazy(Type contractType, Type metadataType = null, string contractName = null);

    }

    #endregion

    #region Composer

    /// <summary>
    /// The one and only internal implementation of IComposer.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class Composer : IComposer
    {
        private const string ModulesDirectoriesConfigKey = "ak.commons.composition.modulesdirectories";

        public CompositionContainer Container { get; private set; }
        public Assembly[] Assemblies { get; private set; }

        public Composer(IAppConfig config, IAppLogger logger)
        {
            var modulesDirectories = config.Get(ModulesDirectoriesConfigKey, string.Empty)
                                           .Split(new[] {';'})
                                           .Select(x => x.ToLower())
                                           .ToList();

            var entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var entryAssemblyDirectory = (Path.GetDirectoryName(entryAssembly.Location) ?? string.Empty).ToLower();

            if (!string.IsNullOrWhiteSpace(entryAssemblyDirectory) &&
                !modulesDirectories.Contains(entryAssemblyDirectory))
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
            this.Assemblies = assemblyCatalogs.Select(x => x.Assembly).ToArray();
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

        public TContract Resolve<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc,
                                                       string contractName = null)
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

        public Lazy<TContract> ResolveLazy<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc,
                                                                 string contractName = null)
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

        public IEnumerable<TContract> ResolveMany<TContract, TMetadata>(Func<TMetadata, bool> metadataFilterFunc,
                                                                        string contractName = null)
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

        public IEnumerable<Lazy<TContract>> ResolveManyLazy<TContract, TMetadata>(
            Func<TMetadata, bool> metadataFilterFunc, string contractName = null)
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

        public IEnumerable<Lazy<object, object>> ResolveManyLazy(Type contractType, Type metadataType = null,
                                                                 string contractName = null)
        {
            return this.Container.GetExports(contractType, metadataType, contractName ?? string.Empty);
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
    }

    #endregion

    #endregion
}