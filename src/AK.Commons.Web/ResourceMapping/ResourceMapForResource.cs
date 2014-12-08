/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResourceMapForResource
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
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    #region IResourceMapForResource

    /// <summary>
    /// Lets you fluently configure mappings for a single resource.
    /// </summary>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TId">Resource ID type.</typeparam>
    /// <typeparam name="TQuery">Resource list query type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> where TService : class
    {
        /// <summary>
        /// Fluently defines mappings for a given resource.
        /// </summary>
        /// <typeparam name="TResourceNext">Resource type.</typeparam>
        /// <typeparam name="TServiceNext">Service type.</typeparam>
        /// <typeparam name="TIdNext">Resource ID type.</typeparam>
        /// <typeparam name="TQueryNext">Resource list query type.</typeparam>
        /// <param name="name">Name of resource.</param>
        /// <returns>Interface that lets you fluently define further configuration for this resource.</returns>
        IResourceMapForResource<TResourceNext, TServiceNext, TIdNext, TQueryNext, TUserId>
            Resource<TResourceNext, TServiceNext, TIdNext, TQueryNext>(string name) where TServiceNext : class;

        /// <summary>
        /// Defines the name of the resource that will serve the link list (i.e. the REST index).
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<NavigationLink, INavigationLinkProvider, int, EmptyQuery, TUserId> Links(string name);

        /// <summary>
        /// Defines a mapping for GET requests.
        /// </summary>
        /// <param name="getAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Get(
            Func<GetDeleteResourceRequest<TService, TId, TUserId>, OperationResult<TResource>> getAccessor);

        /// <summary>
        /// Defines a mapping for POST requests.
        /// </summary>
        /// <param name="postAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Post(
            Func<PostPutResourceRequest<TService, TResource, TUserId>, OperationResult<TResource>> postAccessor);

        /// <summary>
        /// Defines a mapping for PUT requests.
        /// </summary>
        /// <param name="putAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Put(
            Func<PostPutResourceRequest<TService, TResource, TUserId>, OperationResult<TResource>> putAccessor);

        /// <summary>
        /// Defines a mapping for DELETE requests.
        /// </summary>
        /// <param name="deleteAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Delete(
            Func<GetDeleteResourceRequest<TService, TId, TUserId>, OperationResult> deleteAccessor);

        /// <summary>
        /// Defines a mapping for GET requests that ask for lists.
        /// </summary>
        /// <param name="getListAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> GetList(
            Func<GetListResourceRequest<TService, TQuery, TUserId>, OperationResult<IReadOnlyCollection<TResource>>>
                getListAccessor);

        /// <summary>
        /// Defines a mapping for POST requests that ask for lists.
        /// </summary>
        /// <param name="postListAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> PostList(
            Func<PostPutListResourceRequest<TService, TResource, TUserId>, OperationResults<TResource>> postListAccessor);

        /// <summary>
        /// Defines a mapping for PUT requests that ask for lists.
        /// </summary>
        /// <param name="putListAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> PutList(
            Func<PostPutListResourceRequest<TService, TResource, TUserId>, OperationResults<TResource>> putListAccessor);

        /// <summary>
        /// Defines a mapping for DELETE requests that ask for lists.
        /// </summary>
        /// <param name="deleteListAccessor">Function that serves the request.</param>
        /// <returns>Self reference to define further mappings fluently.</returns>
        IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> DeleteList(
            Func<DeleteListResourceRequest<TService, TId, TUserId>, OperationResults> deleteListAccessor);

        /// <summary>
        /// Gets the IResourceProvider instance that corresponds to the resource mapping so far defined.
        /// </summary>
        /// <returns>IResourceProvider instance that can be set to ResourceMapConfiguration.Provider.</returns>
        IResourceProvider GetResourceProvider();
    }

    #endregion

    #region ResourceMapForResource

    /// <summary>
    /// The one and only implementation of IResourceMapForResource.
    /// </summary>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <typeparam name="TId">Resource ID type.</typeparam>
    /// <typeparam name="TQuery">Resource list query type.</typeparam>
    /// <typeparam name="TUserId">User ID type.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class ResourceMapForResource<TResource, TService, TId, TQuery, TUserId> :
        ResourceMapForUser<TUserId>, IResourceMapForResource<TResource, TService, TId, TQuery, TUserId>
        where TService : class
    {
        public string Name { get; private set; }

        public ResourceMapForResource(string name, ResourceMapForUser<TUserId> source)
            : base(source, source.UserIdAccessor)
        {
            this.Name = name;
        }

        IResourceMapForResource<TResourceNext, TServiceNext, TIdNext, TQueryNext, TUserId>
            IResourceMapForResource<TResource, TService, TId, TQuery, TUserId>.Resource
            <TResourceNext, TServiceNext, TIdNext, TQueryNext>(string name)
        {
            return new ResourceMapForResource<TResourceNext, TServiceNext, TIdNext, TQueryNext, TUserId>(name, this);
        }

        public IResourceMapForResource<NavigationLink, INavigationLinkProvider, int, EmptyQuery, TUserId> Links(
            string name)
        {
            return this.Resource<NavigationLink, INavigationLinkProvider, int, EmptyQuery>(name)
                       .GetList(x => x.Service.GetNavigationLinks());
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Get(
            Func<GetDeleteResourceRequest<TService, TId, TUserId>, OperationResult<TResource>> getAccessor)
        {
            if (getAccessor == null) throw new ArgumentNullException("getAccessor");

            var pair = GetResourceOperationKeyValuePair(
                this.Name, HttpMethod.Get, false,
                (resourceProvider, input, context) =>
                    {
                        var service = GetServiceInstance<TService>(resourceProvider);
                        if (service == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                        var id = GetId<TId>(input);
                        var userId = default(TUserId);
                        try
                        {
                            userId = GetUserId<TUserId>(resourceProvider, context);
                        }
                        catch (HttpException exception)
                        {
                            if (exception.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
                                return context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                        }

                        var request = new GetDeleteResourceRequest<TService, TId, TUserId>
                            {
                                Service = service,
                                Id = id,
                                UserId = userId,
                                Context = context
                            };

                        var response = getAccessor(request);
                        return response.GetResponseMessage(resourceProvider, HttpMethod.Get, context);
                    });

            return new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(this.Name, this)
                {
                    GetAccessorKey = pair.Key,
                    GetInvoker = pair.Value
                };
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Post(
            Func<PostPutResourceRequest<TService, TResource, TUserId>, OperationResult<TResource>> postAccessor)
        {
            if (postAccessor == null) throw new ArgumentNullException("postAccessor");

            return this.PostOrPut(HttpMethod.Post, postAccessor);
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Put(
            Func<PostPutResourceRequest<TService, TResource, TUserId>, OperationResult<TResource>> putAccessor)
        {
            if (putAccessor == null) throw new ArgumentNullException("putAccessor");

            return this.PostOrPut(HttpMethod.Put, putAccessor);
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> Delete(
            Func<GetDeleteResourceRequest<TService, TId, TUserId>, OperationResult> deleteAccessor)
        {
            if (deleteAccessor == null) throw new ArgumentNullException("deleteAccessor");

            var pair = GetResourceOperationKeyValuePair(
                this.Name, HttpMethod.Delete, false,
                (resourceProvider, input, context) =>
                    {
                        var service = GetServiceInstance<TService>(resourceProvider);
                        if (service == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                        var id = GetId<TId>(input);
                        var userId = default(TUserId);
                        try
                        {
                            userId = GetUserId<TUserId>(resourceProvider, context);
                        }
                        catch (HttpException exception)
                        {
                            if (exception.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
                                return context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                        }

                        var request = new GetDeleteResourceRequest<TService, TId, TUserId>
                            {
                                Service = service,
                                Id = id,
                                UserId = userId,
                                Context = context
                            };

                        var response = deleteAccessor(request);
                        return response.GetResponseMessage(resourceProvider, HttpMethod.Delete, context);
                    });

            return new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(this.Name, this)
                {
                    DeleteAccessorKey = pair.Key,
                    DeleteInvoker = pair.Value
                };
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> GetList(
            Func<GetListResourceRequest<TService, TQuery, TUserId>,
                OperationResult<IReadOnlyCollection<TResource>>> getListAccessor)
        {
            if (getListAccessor == null) throw new ArgumentNullException("getListAccessor");

            var pair = GetResourceOperationKeyValuePair(
                this.Name, HttpMethod.Get, true,
                (resourceProvider, input, context) =>
                    {
                        var service = GetServiceInstance<TService>(resourceProvider);
                        if (service == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                        var query = JsonUtility.Deserialize<TQuery>(input);
                        var userId = default(TUserId);
                        try
                        {
                            userId = GetUserId<TUserId>(resourceProvider, context);
                        }
                        catch (HttpException exception)
                        {
                            if (exception.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
                                return context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                        }

                        var request = new GetListResourceRequest<TService, TQuery, TUserId>
                            {
                                Service = service,
                                Query = query,
                                UserId = userId,
                                Context = context
                            };

                        var response = getListAccessor(request);
                        return response.GetResponseMessage(resourceProvider, HttpMethod.Get, context);
                    });

            return new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(this.Name, this)
                {
                    GetListAccessorKey = pair.Key,
                    GetListInvoker = pair.Value
                };
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> PostList(
            Func<PostPutListResourceRequest<TService, TResource, TUserId>, OperationResults<TResource>> postListAccessor)
        {
            if (postListAccessor == null) throw new ArgumentNullException("postListAccessor");

            return this.PostOrPutList(HttpMethod.Post, postListAccessor);
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> PutList(
            Func<PostPutListResourceRequest<TService, TResource, TUserId>, OperationResults<TResource>> putListAccessor)
        {
            if (putListAccessor == null) throw new ArgumentNullException("putListAccessor");

            return this.PostOrPutList(HttpMethod.Put, putListAccessor);
        }

        private IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> PostOrPut(
            HttpMethod method,
            Func<PostPutResourceRequest<TService, TResource, TUserId>, OperationResult<TResource>> accessor)
        {
            var pair = GetResourceOperationKeyValuePair(
                this.Name, method, false,
                (resourceProvider, input, context) =>
                    {
                        var service = GetServiceInstance<TService>(resourceProvider);
                        if (service == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                        var resource = JsonUtility.Deserialize<TResource>(input);
                        var userId = default(TUserId);
                        try
                        {
                            userId = GetUserId<TUserId>(resourceProvider, context);
                        }
                        catch (HttpException exception)
                        {
                            if (exception.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
                                return context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                        }

                        var request = new PostPutResourceRequest<TService, TResource, TUserId>
                            {
                                Service = service,
                                Resource = resource,
                                UserId = userId,
                                Context = context
                            };

                        var response = accessor(request);
                        return response.GetResponseMessage(resourceProvider, method, context);
                    });

            var resourceMap = new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(this.Name, this);

            if (method == HttpMethod.Post)
            {
                resourceMap.PostAccessorKey = pair.Key;
                resourceMap.PostInvoker = pair.Value;
            }
            else if (method == HttpMethod.Put)
            {
                resourceMap.PutAccessorKey = pair.Key;
                resourceMap.PutInvoker = pair.Value;
            }

            return resourceMap;
        }

        public IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> DeleteList(
            Func<DeleteListResourceRequest<TService, TId, TUserId>, OperationResults> deleteListAccessor)
        {
            if (deleteListAccessor == null) throw new ArgumentNullException("deleteListAccessor");

            var pair = GetResourceOperationKeyValuePair(
                this.Name, HttpMethod.Delete, true,
                (resourceProvider, input, context) =>
                    {
                        var service = GetServiceInstance<TService>(resourceProvider);
                        if (service == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                        var ids = JsonUtility.Deserialize<IReadOnlyCollection<TId>>(input);
                        var userId = default(TUserId);
                        try
                        {
                            userId = GetUserId<TUserId>(resourceProvider, context);
                        }
                        catch (HttpException exception)
                        {
                            if (exception.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
                                return context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                        }

                        var request = new DeleteListResourceRequest<TService, TId, TUserId>
                            {
                                Service = service,
                                Ids = ids,
                                UserId = userId,
                                Context = context
                            };

                        var responses = deleteListAccessor(request);
                        return responses.GetResponseMessage(HttpMethod.Delete, context);
                    });

            return new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(this.Name, this)
                {
                    DeleteListAccessorKey = pair.Key,
                    DeleteListInvoker = pair.Value
                };
        }

        private IResourceMapForResource<TResource, TService, TId, TQuery, TUserId> PostOrPutList(
            HttpMethod method,
            Func<PostPutListResourceRequest<TService, TResource, TUserId>, OperationResults<TResource>> accessor)
        {
            var pair = GetResourceOperationKeyValuePair(
                this.Name, method, true,
                (resourceProvider, input, context) =>
                    {
                        var service = GetServiceInstance<TService>(resourceProvider);
                        if (service == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                        var resources = JsonUtility.Deserialize<IReadOnlyCollection<TResource>>(input);
                        var userId = default(TUserId);
                        try
                        {
                            userId = GetUserId<TUserId>(resourceProvider, context);
                        }
                        catch (HttpException exception)
                        {
                            if (exception.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
                                return context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                        }

                        var request = new PostPutListResourceRequest<TService, TResource, TUserId>
                            {
                                Service = service,
                                Resources = resources,
                                UserId = userId,
                                Context = context
                            };

                        var responses = accessor(request);
                        return responses.GetResponseMessage(method, context);
                    });

            var resourceMap = new ResourceMapForResource<TResource, TService, TId, TQuery, TUserId>(this.Name, this);

            if (method == HttpMethod.Post)
            {
                resourceMap.PostListAccessorKey = pair.Key;
                resourceMap.PostListInvoker = pair.Value;
            }
            else if (method == HttpMethod.Put)
            {
                resourceMap.PutListAccessorKey = pair.Key;
                resourceMap.PutListInvoker = pair.Value;
            }

            return resourceMap;
        }

        public IResourceProvider GetResourceProvider()
        {
            var resourceProvider = new ResourceProvider();
            BuildResourceProvider(this, resourceProvider);

            return resourceProvider;
        }

        private static void BuildResourceProvider(ResourceMapForUser<TUserId> resourceMap,
                                                  ResourceProvider resourceProvider)
        {
            if (resourceMap.ServiceAccessorType != null && resourceMap.ServiceAccessor != null)
            {
                resourceProvider.ServiceAccessorMap[resourceMap.ServiceAccessorType] =
                    resourceMap.ServiceAccessor;
            }

            resourceProvider.UserIdAccessor = context => resourceMap.UserIdAccessor(context);
            resourceProvider.ServiceFactoryAccessor = resourceMap.ServiceFactoryAccessor ??
                                                      resourceProvider.ServiceFactoryAccessor;
            resourceProvider.ServiceAccessorMap = resourceProvider.ServiceAccessorMap ??
                                                  new Dictionary<Type, Func<object>>();
            resourceProvider.StatusCodeAccessor = resourceMap.StatusCodeAccessor ?? resourceProvider.StatusCodeAccessor;
            resourceProvider.InvokerMap = resourceProvider.InvokerMap ??
                                          new Dictionary<ResourceOperationKey, ResourceOperationInvoker>();

            AssignInvoker(resourceMap, resourceProvider, x => x.GetAccessorKey, x => x.GetInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.PostAccessorKey, x => x.PostInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.PutAccessorKey, x => x.PutInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.DeleteAccessorKey, x => x.DeleteInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.GetListAccessorKey, x => x.GetListInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.PostListAccessorKey, x => x.PostListInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.PutListAccessorKey, x => x.PutListInvoker);
            AssignInvoker(resourceMap, resourceProvider, x => x.DeleteListAccessorKey, x => x.DeleteListInvoker);

            if (resourceMap.Source == null) return;
            BuildResourceProvider(resourceMap.Source, resourceProvider);
        }

        private static void AssignInvoker(ResourceMapForUser<TUserId> resourceMap,
                                          ResourceProvider resourceProvider,
                                          Func<ResourceMapForUser<TUserId>, ResourceOperationKey> keyAccessor,
                                          Func<ResourceMapForUser<TUserId>, ResourceOperationInvoker>
                                              invokerAccessor)
        {
            var invoker = invokerAccessor(resourceMap);
            if (invoker == null) return;

            resourceProvider.InvokerMap[keyAccessor(resourceMap)] = invoker;
        }

        private static T GetId<T>(string input)
        {
            return (T) Convert.ChangeType(input, typeof (T));
        }

        private static T GetUserId<T>(IResourceProvider resourceProvider, HttpControllerContext context)
        {
            return (T) resourceProvider.GetUserId(context);
        }

        private static T GetServiceInstance<T>(IResourceProvider resourceProvider) where T : class
        {
            return typeof (T) == typeof (INavigationLinkProvider)
                       ? (T) NavigationLinkProvider.Instance
                       : (T) resourceProvider.GetServiceInstance(typeof (T));
        }

        private static KeyValuePair<ResourceOperationKey, ResourceOperationInvoker> GetResourceOperationKeyValuePair(
            string resourceName, HttpMethod method, bool isList,
            Func<IResourceProvider, string, HttpControllerContext, HttpResponseMessage> responseAccessor)
        {
            var key = new ResourceOperationKey {Name = resourceName, Method = method, IsList = isList};

            var invoker = new ResourceOperationInvoker(responseAccessor);

            return new KeyValuePair<ResourceOperationKey, ResourceOperationInvoker>(key, invoker);
        }
    }

    #endregion
}