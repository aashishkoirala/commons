/*******************************************************************************************************************************
 * AK.Commons.Configuration.IConfigStore
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

namespace AK.Commons.Configuration
{
    /// <summary>
    /// Interface to be implemented by configuration stores. Creating a configuration store involves implementing this interface,
    /// and then configuring it to be passed in to AppEnvironment.Initialize.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IConfigStore
    {
        /// <summary>
        /// Gets the full text of the configuration XML that follows the standard .NET configuration schema.
        /// </summary>
        /// <returns>Configuration XML string.</returns>
        string GetConfigurationXml();
    }
}