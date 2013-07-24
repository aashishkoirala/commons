/*******************************************************************************************************************************
 * AK.Commons.Composition.IComposer
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

#endregion

namespace AK.Commons.Composition
{
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
}