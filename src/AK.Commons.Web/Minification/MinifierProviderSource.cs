/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.MinifierProviderSource
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Web Library (AKCWL).
 *  
 * AKCWL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCWL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCWL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using AK.Commons.Composition;
using System;

#endregion

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// One and only implementation of IProviderSource&lt;IMinifier&gt;.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class MinifierProviderSource : IProviderSource<IMinifier>
    {
        private readonly IComposer composer;

        public MinifierProviderSource(IComposer composer)
        {
            this.composer = composer;
        }

        /// <summary>
        /// Gets the minifier given the name.
        /// </summary>
        /// <param name="name">One of "js", "css" or "html".</param>
        /// <returns>The corresponding minifier.</returns>
        public IMinifier this[string name]
        {
            get { return this.composer.Resolve<IMinifier, IProviderMetadata>(x => x.Provider == name); }
        }

        /// <summary>
        /// There is no such thing as a default minifier, do not use.
        /// </summary>
        public IMinifier Default
        {
            get { throw new NotSupportedException("There is no such thing as a default minifier."); }
        }
    }
}