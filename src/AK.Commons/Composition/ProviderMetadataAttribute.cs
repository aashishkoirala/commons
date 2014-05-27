/*******************************************************************************************************************************
 * AK.Commons.Composition.ProviderMetadataAttribute
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

using System;
using System.ComponentModel.Composition;

#endregion

namespace AK.Commons.Composition
{
    /// <summary>
    /// Metadata attribute to be applied to provider implementations to
    /// specify provider attributes.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [MetadataAttribute]
    public class ProviderMetadataAttribute : Attribute, IProviderMetadata
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="provider">Unique provider name.</param>
        public ProviderMetadataAttribute(string provider)
        {
            this.Provider = provider;
        }

        /// <summary>
        /// Gets or sets the unique provider name for this implementation.
        /// </summary>
        public string Provider { get; private set; }
    }
}