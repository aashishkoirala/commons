/*******************************************************************************************************************************
 * AK.Commons.InitializationOptions
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

using AK.Commons.Configuration;

namespace AK.Commons
{
    /// <summary>
    /// Contains application environment initialization options.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class InitializationOptions
    {
        public InitializationOptions()
        {
            this.EnableLogging = true;
            this.GenerateServiceClients = true;
        }

        /// <summary>
        /// A valid IConfigStore instance, or unset (null) if one does
        /// not wish to use configuration.
        /// </summary>
        public IConfigStore ConfigStore { get; set; }

        /// <summary>
        /// Whether to enable logging (true by default), requires
        /// ConfigStore to be specified.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Whether to autogenerate WCF client proxy code, requires
        /// ConfigStore to be specified.
        /// </summary>
        public bool GenerateServiceClients { get; set; }

        /// <summary>
        /// Temp location to use to store configuration store data
        /// during initialization. If nothing is provided, the default
        /// TEMP environment variable-pointed location is used.
        /// </summary>
        public string ConfigTempLocation { get; set; }
    }
}