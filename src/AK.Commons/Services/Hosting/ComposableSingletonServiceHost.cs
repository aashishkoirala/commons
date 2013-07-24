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

namespace AK.Commons.Services.Hosting
{
    public class ComposableSingletonServiceHost : ServiceHost
    {
        public ComposableSingletonServiceHost(Type contractType, params Uri[] baseAddresses) : base(AppEnvironment.Composer.Resolve(contractType), baseAddresses) { }
    }

    public class ComposableSingletonServiceHost<TContract> : ServiceHost
    {
        public ComposableSingletonServiceHost(params Uri[] baseAddresses) : base(AppEnvironment.Composer.Resolve<TContract>(), baseAddresses) {}
    }
}