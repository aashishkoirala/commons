/*******************************************************************************************************************************
 * AK.Commons.Services.Behaviors.RestrictIpServiceBehavior
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
 * The work in this particular file borrows from the following:
 * http://www.codeproject.com/Articles/37280/WCF-Service-Behavior-Example-IPFilter-Allow-Deny-A
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

#endregion

namespace AK.Commons.Services.Behaviors
{
    /// <summary>
    /// WCF service behavior that restricts callers within a specific IP value set. 
    /// </summary>
    /// <author>
    /// Aashish Koirala, but also largely borrowed from content available at:
    /// http://www.codeproject.com/Articles/37280/WCF-Service-Behavior-Example-IPFilter-Allow-Deny-A
    /// </author>
    public class RestrictIpServiceBehavior : IServiceBehavior, IDispatchMessageInspector
    {
        private static readonly object httpAccessDenied = new object();
        private static readonly object nonHttpAccessDenied = new object();

        private readonly string[] allowedIps;
        private readonly bool allowLocalhost;

        public RestrictIpServiceBehavior(string[] allowedIps, bool allowLocalhost)
        {
            this.allowedIps = allowedIps;
            this.allowLocalhost = allowLocalhost;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var endpointDispatchers = serviceHostBase
                .ChannelDispatchers
                .OfType<ChannelDispatcher>()
                .SelectMany(x => x.Endpoints);

            foreach (var endpointDispatcher in endpointDispatchers)
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var remoteEndpoint = (RemoteEndpointMessageProperty) request.Properties[RemoteEndpointMessageProperty.Name];

            if (this.allowedIps.Contains(remoteEndpoint.Address)) return null;
            if (this.allowLocalhost && remoteEndpoint.Address.StartsWith("127.0.0.")) return null;

            // ReSharper disable AssignNullToNotNullAttribute
            request = null;
            // ReSharper restore AssignNullToNotNullAttribute

            var scheme = channel.LocalAddress.Uri.Scheme;
            return scheme.Equals(Uri.UriSchemeHttp) || scheme.Equals(Uri.UriSchemeHttps)
                       ? httpAccessDenied
                       : nonHttpAccessDenied;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState != httpAccessDenied) return;

            reply.Properties["httpResponse"] = new HttpResponseMessageProperty
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
        }
    }
}