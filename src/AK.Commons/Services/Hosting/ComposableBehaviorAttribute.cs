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
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace AK.Commons.Services.Hosting
{
    public class ComposableBehaviorAttribute : ServiceMetadataAttribute, IContractBehavior, IContractBehaviorAttribute
    {
        public Type TargetContract
        {
            get { return null; }
        }

        public void AddBindingParameters(ContractDescription description,
                                         ServiceEndpoint endpoint, BindingParameterCollection parameters) {}

        public void ApplyClientBehavior(ContractDescription description,
                                        ServiceEndpoint endpoint, ClientRuntime clientRuntime) {}

        public void ApplyDispatchBehavior(ContractDescription description,
                                          ServiceEndpoint endpoint, DispatchRuntime dispatch)
        {
            dispatch.InstanceProvider = new CompositionInstanceProvider(description.ContractType);
        }

        public void Validate(ContractDescription description, ServiceEndpoint endpoint) {}

        public override bool IsClient { get { return false; } }

        public override bool IsService { get { return true; } }
    }
}