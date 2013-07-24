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
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace AK.Commons.Services.Hosting
{
    public class ComposableServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type contractType, Uri[] baseAddresses)
        {
            return new ComposableServiceHost(contractType, baseAddresses);
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            // TODO: Implement.
            //
            throw new NotImplementedException();
        }

        public ServiceHost CreateServiceHost<TContract>(params Uri[] baseAddresses)
        {
            return new ComposableServiceHost<TContract>(baseAddresses);
        }

        public ServiceHost CreateSingletonServiceHost<TContract>(params Uri[] baseAddresses)
        {
            return new ComposableSingletonServiceHost<TContract>(baseAddresses);
        }
    }
}