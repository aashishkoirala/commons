/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResourceMapConfiguration
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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Provides ways to configure the resource map.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class ResourceMapConfiguration
    {
        private static readonly List<NavigationLink> navigationLinks = new List<NavigationLink>();

        internal static IReadOnlyCollection<NavigationLink> NavigationLinks
        {
            get { return navigationLinks.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the currently configured IResourceProvider instance that represents the resource mappings.
        /// </summary>
        public static IResourceProvider Provider { get; set; }

        /// <summary>
        /// Gets an interface that lets you define navigation link entries (i.e. the REST index) fluently.
        /// </summary>
        /// <param name="routeName">Route for which to define navigation links.</param>
        /// <returns>LinkHelper object - lets you fluently define navigation links.</returns>
        public static LinkHelper Route(string routeName)
        {
            return new LinkHelper(routeName);
        }

        private static void AddNavigationLink(
            string rel, string routeName, string resourceName, HttpMethod method, bool isList)
        {
            var href = GlobalConfiguration.Configuration.Routes[routeName]
                .RouteTemplate
                .Replace("{resource}", resourceName)
                .Replace("{id}", isList ? string.Empty : ":id");

            href = IsAbsoluteUrl(href) ? href : VirtualPathUtility.ToAbsolute("~/") + href;

            navigationLinks.Add(new NavigationLink
                {
                    Rel = rel,
                    Href = href,
                    Method = method.ToString().ToUpper(),
                    IsArray = isList
                });
        }

        private static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public class LinkHelper
        {
            private readonly string routeName;

            public LinkHelper(string routeName)
            {
                this.routeName = routeName;
            }

            /// <summary>
            /// Adds the given navigation link entry to the configuration.
            /// </summary>
            /// <param name="rel">Name of link.</param>
            /// <param name="resourceName">Name of resource.</param>
            /// <param name="method">HTTP method.</param>
            /// <param name="isList">Whether link represents array instead of object.</param>
            /// <returns>Reference to interface to fluently define another link.</returns>
            public LinkHelper Link(string rel, string resourceName, HttpMethod method,
                                   bool isList = false)
            {
                AddNavigationLink(rel, this.routeName, resourceName, method, isList);
                return this;
            }
        }
    }
}