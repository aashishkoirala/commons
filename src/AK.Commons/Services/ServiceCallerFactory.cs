/*******************************************************************************************************************************
 * AK.Commons.Services.ServiceCallerFactory
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
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
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

#endregion

namespace AK.Commons.Services
{
    /// <summary>
    /// Creates instances of IServiceCaller[T] which you can use to call WCF operations on contracts of type T.
    /// The instance returned handles channel opening/closing.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class ServiceCallerFactory
    {
        private static readonly object serviceCallerMapLock = new object();

        private static readonly IDictionary<Type, ServiceCaller> serviceCallerMap =
            new Dictionary<Type, ServiceCaller>();

        /// <summary>
        /// A function to return a ServiceEndpoint based on (or for all) channel types.
        /// If you leave this unassigned, the default behavior (i.e. scanning of config files)
        /// will be used when Create is called. Within the function, you can use the methods
        /// in <see cref="EndpointFactory"/> to get the ServiceEndpoint from other objects.
        /// </summary>
        public static Func<Type, ServiceEndpoint> ServiceEndpointAccessor { get; set; }

        /// <summary>
        /// Creates an IServiceCaller[T] based on the definition of ServiceEndpointAccessor.
        /// </summary>
        /// <typeparam name="TChannel">WCF channel contract type.</typeparam>
        /// <returns>IServiceCaller[T] instance.</returns>
        public static IServiceCaller<TChannel> Create<TChannel>(EndpointAddress endpointAddress = null)
        {
            ServiceCaller caller;
            if (serviceCallerMap.TryGetValue(typeof (TChannel), out caller))
                return (IServiceCaller<TChannel>) caller;

            lock (serviceCallerMapLock)
            {
                if (serviceCallerMap.TryGetValue(typeof (TChannel), out caller))
                    return (IServiceCaller<TChannel>) caller;
                
                if (ServiceEndpointAccessor != null)
                {
                    var serviceEndpoint = ServiceEndpointAccessor(typeof (TChannel));
                    if (endpointAddress != null) serviceEndpoint.Address = endpointAddress;
                    caller = new ServiceCaller<TChannel>(serviceEndpoint);
                }
                else
                {
                    caller = endpointAddress == null
                                 ? new ServiceCaller<TChannel>()
                                 : new ServiceCaller<TChannel>(endpointAddress);                    
                }

                serviceCallerMap[typeof (TChannel)] = caller;
            }

            return (IServiceCaller<TChannel>) caller;
        }
    }
}