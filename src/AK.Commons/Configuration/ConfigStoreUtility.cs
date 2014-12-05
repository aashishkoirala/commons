/*******************************************************************************************************************************
 * AK.Commons.Configuration.ConfigStoreUtility
 * Copyright © 2013-2014 Aashish Koirala <http://aashishkoirala.github.io>
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

using AK.Commons.Configuration.Sections;
using AK.Commons.Exceptions;
using System;
using SC = System.Configuration;

#endregion

namespace AK.Commons.Configuration
{
    /// <summary>
    /// Exposes utility and extension methods having to do with the configuration store mechanism.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class ConfigStoreUtility
    {
        private const string ConfigStoreSectionName = "ak.commons.configuration.store";

        /// <summary>
        /// Gets an instance of IConfigStore using the given loaded configuration.
        /// </summary>
        /// <param name="configuration">Configuration object.</param>
        /// <returns>Instance of IConfigStore.</returns>
        /// <exception cref="AppConfigException">If there's an issue retrieving the configuration store settings.</exception>
        public static IConfigStore GetConfigStore(this SC.Configuration configuration)
        {
            IConfigStore configStore = null;
            
            try
            {
                var configStoreSection = configuration.GetSection(ConfigStoreSectionName) as StoreConfigurationSection;
                if (configStoreSection == null)
                    throw new AppConfigException(AppConfigExceptionReason.ConfigStoreError, "Configuration store section not found.");

                configStore = configStoreSection.Store.GetObject() as IConfigStore;
                if (configStore == null)
                    throw new AppConfigException(AppConfigExceptionReason.ConfigStoreError, "Configuration store is not of type IConfigStore.");

                return configStore;
            }
            catch (AppConfigException)
            {
                throw;
            }
            catch(Exception exception)
            {
                exception.WrapAndThrow<AppConfigException, AppConfigExceptionReason>(AppConfigExceptionReason.ConfigStoreError);
            }

            return configStore;
        }
    }
}