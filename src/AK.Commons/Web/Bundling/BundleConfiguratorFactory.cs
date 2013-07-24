/*******************************************************************************************************************************
 * AK.Commons.Web.Bundling.BundleConfiguratorFactory
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

#region Namespace Imports

using AK.Commons.Composition;
using AK.Commons.Configuration;

#endregion

namespace AK.Commons.Web.Bundling
{
    /// <summary>
    /// Creates IBundleConfigurator instances based on provided IAppConfig.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal static class BundleConfiguratorFactory
    {
        private const string ConfigProviderNameKey = "ak.commons.web.bundling.provider";

        public static IBundleConfigurator Create(IComposer composer, IAppConfig appConfig)
        {
            var provider = appConfig.Get<string>(ConfigProviderNameKey);
            return composer.Resolve<IBundleConfigurator, IProviderMetadata>(metadata => metadata.Provider == provider);
        }
    }
}