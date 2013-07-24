/*******************************************************************************************************************************
 * AK.Commons.Services
 * 
 * THIS NAMESPACE IS UNDER DEVELOPMENT.
 * 
 * TODO: Build WCF library within AK.Common.Services.
 * 
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
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

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using AK.Commons.Services.Client;

namespace AK.Commons.Services.Hosting
{
    public class ComposableServiceHost : ServiceHost
    {
        public ComposableServiceHost(Type contractType, params Uri[] baseAddresses) : base(GetServiceType(contractType), baseAddresses) {}

        private static Type GetServiceType(Type contractType)
        {
            var exports = AppEnvironment.Composer.ResolveManyLazy(contractType, typeof (IServiceMetadata));
            return exports.Single(x => ((IServiceMetadata) x.Metadata).IsService).Value.GetType();
        }
    }

    public class ComposableServiceHost<TContract> : ServiceHost
    {
        public ComposableServiceHost(params Uri[] baseAddresses) : 
            base(AppEnvironment.Composer.Resolve<TContract, IServiceMetadata>(metadata => metadata.IsService).GetType(), baseAddresses) {}

        private static ServiceEndpoint CreateEndpoint(ServiceClientConfiguration hostingConfiguration, ContractDescription contractDescription)
        {
            var binding = hostingConfiguration.BindingConfiguration.Instantiate<Binding>();
            var endpointAddress = new EndpointAddress(hostingConfiguration.EndpointAddressUri);

            return new ServiceEndpoint(contractDescription, binding, endpointAddress);
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            var serviceConfiguration = AppEnvironment.Config.GetServiceConfiguration();
            var hostingConfiguration = serviceConfiguration.GetHostingConfiguration<TContract>();

            this.ImplementedContracts.Values.ForEach(x =>
            {
                var endpoint = CreateEndpoint(hostingConfiguration, x);
                this.AddServiceEndpoint(endpoint);
            });
        }
    }
}