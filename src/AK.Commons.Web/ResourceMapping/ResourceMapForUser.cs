/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResourceMapForUser
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
using System.Net;
using System.Web.Http.Controllers;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Returns an interface scoped to the given user that lets you fluently define resource mappings.
    /// </summary>
    /// <typeparam name="TUserId">Type of user ID.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IResourceMapForUser<TUserId>
    {
        /// <summary>
        /// Fluently defines a service mapping.
        /// </summary>
        /// <typeparam name="TService">Type of service contract.</typeparam>
        /// <param name="serviceAccessor">Function that returns the service instance.</param>
        /// <returns>Self reference for fluent definition of something else.</returns>
        IResourceMapForUser<TUserId> Service<TService>(Func<TService> serviceAccessor) where TService : class;

        /// <summary>
        /// Fluently defines all service mappings.
        /// </summary>
        /// <param name="serviceFactoryAccessor">A general function that returns a service given a type.</param>
        /// <returns>Self reference for fluent definition of something else.</returns>
        IResourceMapForUser<TUserId> ServiceFactory(Func<Type, object> serviceFactoryAccessor);

        /// <summary>
        /// Fluently defines status code mappings.
        /// </summary>
        /// <param name="statusCodeAccessor">Function that returns an HTTP status code given a error code string.</param>
        /// <returns>Self reference for fluent definition of something else.</returns>
        IResourceMapForUser<TUserId> StatusCode(Func<String, HttpStatusCode> statusCodeAccessor);

        /// <summary>
        /// Fluently defines mappings for a given resource.
        /// </summary>
        /// <typeparam name="TResource">Resource type.</typeparam>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TId">Resource ID type.</typeparam>
        /// <typeparam name="TQuery">Resource list query type.</typeparam>
        /// <param name="name">Name of resource.</param>
        /// <returns>Interface that lets you fluently define further configuration for this resource.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId>
            Resource<TResource, TService, TId, TQuery>(string name) where TService : class;
    }

    /// <summary>
    /// The one and only implementation of IResourceMapForUser.
    /// </summary>
    /// <typeparam name="TUserId">Type of user ID.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class ResourceMapForUser<TUserId> : IResourceMapForUser<TUserId>
    {
        #region Properties

        public ResourceMapForUser<TUserId> Source { get; private set; }
        public Func<HttpControllerContext, TUserId> UserIdAccessor { get; private set; }
        public Type ServiceAccessorType { get; set; }
        public Func<object> ServiceAccessor { get; set; }
        public Func<Type, object> ServiceFactoryAccessor { get; set; }
        public Func<string, HttpStatusCode> StatusCodeAccessor { get; set; }
        public ResourceOperationKey GetAccessorKey { get; set; }
        public ResourceOperationInvoker GetInvoker { get; set; }
        public ResourceOperationKey PostAccessorKey { get; set; }
        public ResourceOperationInvoker PostInvoker { get; set; }
        public ResourceOperationKey PutAccessorKey { get; set; }
        public ResourceOperationInvoker PutInvoker { get; set; }
        public ResourceOperationKey DeleteAccessorKey { get; set; }
        public ResourceOperationInvoker DeleteInvoker { get; set; }
        public ResourceOperationKey GetListAccessorKey { get; set; }
        public ResourceOperationInvoker GetListInvoker { get; set; }
        public ResourceOperationKey PostListAccessorKey { get; set; }
        public ResourceOperationInvoker PostListInvoker { get; set; }
        public ResourceOperationKey PutListAccessorKey { get; set; }
        public ResourceOperationInvoker PutListInvoker { get; set; }
        public ResourceOperationKey DeleteListAccessorKey { get; set; }
        public ResourceOperationInvoker DeleteListInvoker { get; set; }

        #endregion

        public ResourceMapForUser(ResourceMapForUser<TUserId> source,
                                  Func<HttpControllerContext, TUserId> userIdAccessor)
        {
            this.Source = source;
            this.UserIdAccessor = userIdAccessor;
        }

        public IResourceMapForUser<TUserId> Service<TService>(Func<TService> serviceAccessor) where TService : class
        {
            if (serviceAccessor == null) throw new ArgumentNullException("serviceAccessor");

            return new ResourceMapForUser<TUserId>(this, this.UserIdAccessor)
                {
                    ServiceAccessorType = typeof (TService),
                    ServiceAccessor = serviceAccessor
                };
        }

        public IResourceMapForUser<TUserId> ServiceFactory(Func<Type, object> serviceFactoryAccessor)
        {
            if (serviceFactoryAccessor == null) throw new ArgumentNullException("serviceFactoryAccessor");

            return new ResourceMapForUser<TUserId>(this, this.UserIdAccessor)
                {
                    ServiceFactoryAccessor = serviceFactoryAccessor
                };
        }

        public IResourceMapForUser<TUserId> StatusCode(Func<string, HttpStatusCode> statusCodeAccessor)
        {
            if (statusCodeAccessor == null) throw new ArgumentNullException("statusCodeAccessor");

            return new ResourceMapForUser<TUserId>(this, this.UserIdAccessor)
                {
                    StatusCodeAccessor = statusCodeAccessor
                };
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Resource
            <TResource, TService, TId, TQuery>(string name) where TService : class
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            return new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(name, this);
        }
    }
}