/*******************************************************************************************************************************
 * AK.Commons.Services.ServiceCaller
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
using System.ServiceModel;
using System.ServiceModel.Description;

#endregion

namespace AK.Commons.Services
{
    /// <summary>
    /// This is a dumb untyped ancestor for ServiceCaller[T] for use by <see cref="ServiceCallerFactory" />.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ServiceCaller {}

    /// <summary>
    /// The one and only implementation of IServiceCaller[T].
    /// </summary>
    /// <typeparam name="TChannel">WCF channel contract type.</typeparam>
    /// <author>Aashish Koirala</author>
    internal class ServiceCaller<TChannel> : ServiceCaller, IServiceCaller<TChannel>
    {
        private readonly ServiceEndpoint endpoint;
        private readonly EndpointAddress endpointAddress;

        public ServiceCaller() {}

        public ServiceCaller(ServiceEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public ServiceCaller(EndpointAddress endpointAddress)
        {
            this.endpointAddress = endpointAddress;
        }

        public void Call(Action<TChannel> action)
        {
            var channelFactory = this.endpoint != null
                                     ? new ChannelFactory<TChannel>(this.endpoint)
                                     : new ChannelFactory<TChannel>();

            if (this.endpointAddress != null) channelFactory.Endpoint.Address = endpointAddress;

            var channel = channelFactory.CreateChannel();
            try
            {
                action(channel);
            }
            catch
            {
                channelFactory.Abort();
                throw;
            }
            finally
            {
                channelFactory.Close();
            }
        }

        public TResult Call<TResult>(Func<TChannel, TResult> action)
        {
            var channelFactory = this.endpoint != null
                                     ? new ChannelFactory<TChannel>(this.endpoint)
                                     : new ChannelFactory<TChannel>();

            var channel = channelFactory.CreateChannel();
            try
            {
                return action(channel);
            }
            catch
            {
                channelFactory.Abort();
                throw;
            }
            finally
            {
                channelFactory.Close();
            }
        }
    }
}