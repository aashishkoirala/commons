/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.NavigationLink
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

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Represents an entry in the navigation link list provided by the REST index.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class NavigationLink
    {
        /// <summary>
        /// Name of the link.
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// URL of the link.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// HTTP method associated with the link.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Whether the link corresponds to an array resource instead of a single object.
        /// </summary>
        public bool IsArray { get; set; }
    }
}