/*******************************************************************************************************************************
 * AK.Commons.Web.LibraryResources.LibraryUtility
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

using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace AK.Commons.Web.LibraryResources
{
    /// <summary>
    /// Utility methods related to library resources.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class LibraryUtility
    {
        /// <summary>
        /// Maps routes for serving up library-resource content.
        /// </summary>
        /// <param name="routes">Route collection.</param>
        public static void MapLibraryContent(this RouteCollection routes)
        {
            routes.MapRoute("LibraryContent", "lib/{type}/{name}", new
                {
                    controller = "Library",
                    action = "Library"
                });
        }
    }
}