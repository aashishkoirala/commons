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
using AK.Commons.Configuration;
using AK.Commons.Services.Client;

namespace AK.Commons.Services
{
    public class ServiceConfiguration
    {
        private readonly IAppConfig config;

        private const string ClientConfigKey = "_.services.clients.{0}";
        private const string HostConfigKey = "_.services.hosts.{0}";

        public ServiceConfiguration(IAppConfig config)
        {
            this.config = config;
        }

        public ServiceClientConfiguration GetClientConfiguration<TContract>()
        {
            var key = string.Format(ClientConfigKey, typeof (TContract).FullName);

            return this.config.Get<ServiceClientConfiguration>(key);
        }

        public ServiceClientConfiguration GetHostingConfiguration(Type contractType)
        {
            var key = string.Format(HostConfigKey, contractType.FullName);

            return this.config.Get<ServiceClientConfiguration>(key);
        }

        public ServiceClientConfiguration GetHostingConfiguration<TContract>()
        {
            return this.GetHostingConfiguration(typeof (TContract));
        }
    }
}