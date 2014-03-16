/*******************************************************************************************************************************
 * AK.Commons.Services.EndpointFactory
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
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

#endregion

namespace AK.Commons.Services
{
    /// <summary>
    /// Convenience factory methods to create ServiceEndpoint objects based on a variety of combinations as
    /// accepted by the various constructors of ChannelFactory.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class EndpointFactory
    {
        #region Create<TChannel>

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <typeparam name="TChannel">Channel type.</typeparam>
        /// <param name="binding">Binding object.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create<TChannel>(Binding binding)
        {
            return new ChannelFactory<TChannel>(binding).Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <typeparam name="TChannel">Channel type.</typeparam>
        /// <param name="binding">Binding object.</param>
        /// <param name="remoteAddress">Remote endpoint address.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create<TChannel>(Binding binding, EndpointAddress remoteAddress)
        {
            return new ChannelFactory<TChannel>(binding, remoteAddress).Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <typeparam name="TChannel">Channel type.</typeparam>
        /// <param name="binding">Binding object.</param>
        /// <param name="remoteAddress">Remote endpoint address URI.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create<TChannel>(Binding binding, string remoteAddress)
        {
            return new ChannelFactory<TChannel>(binding, remoteAddress).Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <typeparam name="TChannel">Channel type.</typeparam>
        /// <param name="endpointConfigurationName">Name of endpoint configuration in config file.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create<TChannel>(string endpointConfigurationName)
        {
            return new ChannelFactory<TChannel>(endpointConfigurationName).Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <typeparam name="TChannel">Channel type.</typeparam>
        /// <param name="endpointConfigurationName">Name of endpoint configuration in config file.</param>
        /// <param name="remoteAddress">Remote endpoint address.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create<TChannel>(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            return new ChannelFactory<TChannel>(endpointConfigurationName, remoteAddress).Endpoint;
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <param name="channelType">Channel type.</param>
        /// <param name="binding">Binding object.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create(Type channelType, Binding binding)
        {
            var channelFactoryType = typeof (ChannelFactory<>).MakeGenericType(channelType);
            var channelFactoryConstructor = channelFactoryType.GetConstructor(new[] {typeof (Binding)});
            if (channelFactoryConstructor == null) throw new InvalidOperationException();

            var channelFactory = (ChannelFactory) channelFactoryConstructor.Invoke(new object[] {binding});
            return channelFactory.Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <param name="channelType">Channel type.</param>
        /// <param name="binding">Binding object.</param>
        /// <param name="remoteAddress">Remote endpoint address.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create(Type channelType, Binding binding, EndpointAddress remoteAddress)
        {
            var channelFactoryType = typeof (ChannelFactory<>).MakeGenericType(channelType);
            var channelFactoryConstructor =
                channelFactoryType.GetConstructor(new[] {typeof (Binding), typeof (EndpointAddress)});
            if (channelFactoryConstructor == null) throw new InvalidOperationException();

            var channelFactory = (ChannelFactory) channelFactoryConstructor.Invoke(
                new object[] {binding, remoteAddress});
            return channelFactory.Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <param name="channelType">Channel type.</param>
        /// <param name="binding">Binding object.</param>
        /// <param name="remoteAddress">Remote endpoint address URI.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create(Type channelType, Binding binding, string remoteAddress)
        {
            var channelFactoryType = typeof (ChannelFactory<>).MakeGenericType(channelType);
            var channelFactoryConstructor =
                channelFactoryType.GetConstructor(new[] {typeof (Binding), typeof (string)});
            if (channelFactoryConstructor == null) throw new InvalidOperationException();

            var channelFactory = (ChannelFactory) channelFactoryConstructor.Invoke(
                new object[] {binding, remoteAddress});
            return channelFactory.Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <param name="channelType">Channel type.</param>
        /// <param name="endpointConfigurationName">Name of endpoint configuration in config file.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create(Type channelType, string endpointConfigurationName)
        {
            var channelFactoryType = typeof (ChannelFactory<>).MakeGenericType(channelType);
            var channelFactoryConstructor =
                channelFactoryType.GetConstructor(new[] {typeof (string)});
            if (channelFactoryConstructor == null) throw new InvalidOperationException();

            var channelFactory = (ChannelFactory) channelFactoryConstructor.Invoke(
                new object[] {endpointConfigurationName});
            return channelFactory.Endpoint;
        }

        /// <summary>
        /// Creates a new ServiceEndpoint based on the given parameters.
        /// </summary>
        /// <param name="channelType">Channel type.</param>
        /// <param name="endpointConfigurationName">Name of endpoint configuration in config file.</param>
        /// <param name="remoteAddress">Remote endpoint address.</param>
        /// <returns>ServiceEndpoint object.</returns>
        public static ServiceEndpoint Create(
            Type channelType, string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            var channelFactoryType = typeof (ChannelFactory<>).MakeGenericType(channelType);
            var channelFactoryConstructor =
                channelFactoryType.GetConstructor(new[] {typeof (string), typeof (EndpointAddress)});
            if (channelFactoryConstructor == null) throw new InvalidOperationException();

            var channelFactory = (ChannelFactory) channelFactoryConstructor.Invoke(
                new object[] {endpointConfigurationName, remoteAddress});
            return channelFactory.Endpoint;
        }

        #endregion
    }
}