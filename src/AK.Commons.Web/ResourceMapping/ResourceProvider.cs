/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResourceProvider
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
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Interface to be used by ResourceController, etc. to access configured resource mappings. This needs to be assigned
    /// to ResourceMapConfiguration.Provider, and can be obtained from fluently mapped resource mapping.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IResourceProvider
    {
        /// <summary>
        /// Extracts the user ID from the request using the user ID accessor configured in the resource mapping.
        /// </summary>
        /// <param name="context">HTTP controller context.</param>
        /// <returns>User ID.</returns>
        object GetUserId(HttpControllerContext context);

        /// <summary>
        /// Gets the instance of the given type of service using the accessor configured in the resource mapping.
        /// </summary>
        /// <param name="serviceType">Service type (contract).</param>
        /// <returns>Service instance.</returns>
        object GetServiceInstance(Type serviceType);

        /// <summary>
        /// Gets the HTTP status code corresponding to the given error code based on the accessor configured in
        /// the resource mapping.
        /// </summary>
        /// <param name="code">Error code from service.</param>
        /// <returns>HTTP status code</returns>
        HttpStatusCode GetStatusCode(string code);

        /// <summary>
        /// Based on the resource mapping that is configured, serves the resource request.
        /// </summary>
        /// <param name="resourceName">Name of resource.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="isList">Whether we're asking for a list.</param>
        /// <param name="input">Request body.</param>
        /// <param name="context">HTTP controller context.</param>
        /// <returns>HTTP response message</returns>
        HttpResponseMessage GetResponse(string resourceName, HttpMethod method, bool isList, string input,
                                        HttpControllerContext context);
    }

    /// <summary>
    /// The one and only implementation of IResourceProvider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ResourceProvider : IResourceProvider
    {
        public Func<HttpControllerContext, object> UserIdAccessor { get; set; }
        public Func<Type, object> ServiceFactoryAccessor { get; set; }
        public IDictionary<Type, Func<object>> ServiceAccessorMap { get; set; }
        public Func<string, HttpStatusCode> StatusCodeAccessor { get; set; }
        public IDictionary<ResourceOperationKey, ResourceOperationInvoker> InvokerMap { get; set; }

        public object GetUserId(HttpControllerContext context)
        {
            return this.UserIdAccessor == null ? null : this.UserIdAccessor(context);
        }

        public object GetServiceInstance(Type serviceType)
        {
            if (this.ServiceAccessorMap == null && this.ServiceFactoryAccessor == null) return null;

            var perhapsServiceAccessor = this.ServiceAccessorMap.LookFor(serviceType);

            if (perhapsServiceAccessor.IsThere) return perhapsServiceAccessor.Value();

            return this.ServiceFactoryAccessor == null ? null : this.ServiceFactoryAccessor(serviceType);
        }

        public HttpStatusCode GetStatusCode(string code)
        {
            return this.StatusCodeAccessor == null
                       ? HttpStatusCode.InternalServerError
                       : this.StatusCodeAccessor(code);
        }

        public HttpResponseMessage GetResponse(string resourceName, HttpMethod method, bool isList, string input,
                                               HttpControllerContext context)
        {
            var key = new ResourceOperationKey {Name = resourceName, Method = method, IsList = isList};

            if (this.InvokerMap == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            return this.InvokerMap
                       .LookFor(key)
                       .DoIfThere(x => x.ResponseAccessor(this, input, context),
                                  new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }

    /// <summary>
    /// Represents a single resource-operation key.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal struct ResourceOperationKey
    {
        public string Name { get; set; }
        public HttpMethod Method { get; set; }
        public bool IsList { get; set; }
    }

    /// <summary>
    /// Represents a single resource operation invoker.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ResourceOperationInvoker
    {
        public Func<IResourceProvider, string, HttpControllerContext, HttpResponseMessage> ResponseAccessor { get; private set; }

        public ResourceOperationInvoker(
            Func<IResourceProvider, string, HttpControllerContext, HttpResponseMessage> responseAccessor)
        {
            this.ResponseAccessor = responseAccessor;
        }
    }
}