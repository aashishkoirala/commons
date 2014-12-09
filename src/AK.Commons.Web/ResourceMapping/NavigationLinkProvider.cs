/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.NavigationLinkProvider
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

using AK.Commons.Services;
using System.Collections.Generic;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// A singleton service that serves up a list of navigation links, i.e. a REST index.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface INavigationLinkProvider
    {
        /// <summary>
        /// Gets a result encapsulating navigation link list.
        /// </summary>
        /// <returns>Result encapsulating navigation link list.</returns>
        OperationResult<IReadOnlyCollection<NavigationLink>> GetNavigationLinks();
    }

    /// <summary>
    /// The one and only implementation of INavigationLinkProvider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class NavigationLinkProvider : INavigationLinkProvider
    {
        /// <summary>
        /// The one and only INavigationLinkProvider instance.
        /// </summary>
        public static readonly INavigationLinkProvider Instance = new NavigationLinkProvider();

        /// <summary>
        /// Gets a result encapsulating navigation link list.
        /// </summary>
        /// <returns>Result encapsulating navigation link list.</returns>
        public OperationResult<IReadOnlyCollection<NavigationLink>> GetNavigationLinks()
        {
            return new OperationResult<IReadOnlyCollection<NavigationLink>>(ResourceMapConfiguration.NavigationLinks);
        }
    }
}