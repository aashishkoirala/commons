/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResourceRequest
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

using System.Collections.Generic;
using System.Web.Http.Controllers;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Base class for all resource requests.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    /// <author>Aashish Koirala</author>
    public abstract class ResourceRequest<TService, TUserId>
    {
        /// <summary>
        /// Service instance.
        /// </summary>
        public TService Service { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public TUserId UserId { get; set; }

        /// <summary>
        /// HTTP controller context.
        /// </summary>
        public HttpControllerContext Context { get; set; }
    }

    /// <summary>
    /// Represents GET and DELETE requests.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TId">Type of resource ID.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    public class GetDeleteResourceRequest<TService, TId, TUserId> : ResourceRequest<TService, TUserId>
    {
        /// <summary>
        /// Resource ID.
        /// </summary>
        public TId Id { get; set; }
    }

    /// <summary>
    /// Represents a GET request asking for a list.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TQuery">Query type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    public class GetListResourceRequest<TService, TQuery, TUserId> : ResourceRequest<TService, TUserId>
    {
        /// <summary>
        /// List query.
        /// </summary>
        public TQuery Query { get; set; }
    }

    /// <summary>
    /// Represents a POST or PUT request for a single object.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    public class PostPutResourceRequest<TService, TResource, TUserId> : ResourceRequest<TService, TUserId>
    {
        /// <summary>
        /// Resource instance.
        /// </summary>
        public TResource Resource { get; set; }
    }

    /// <summary>
    /// Represents a POST or PUT request for a list of objects.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    public class PostPutListResourceRequest<TService, TResource, TUserId> : ResourceRequest<TService, TUserId>
    {
        /// <summary>
        /// Resource list.
        /// </summary>
        public IReadOnlyCollection<TResource> Resources { get; set; }
    }

    /// <summary>
    /// Represents a DELETE request for a single object.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TId">Resource ID type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    public class DeleteListResourceRequest<TService, TId, TUserId> : ResourceRequest<TService, TUserId>
    {
        /// <summary>
        /// Resource ID list.
        /// </summary>
        public IReadOnlyCollection<TId> Ids { get; set; }
    }
}