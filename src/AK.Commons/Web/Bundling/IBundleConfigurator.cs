/*******************************************************************************************************************************
 * AK.Commons.Web.Bundling.IBundleConfigurator
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

namespace AK.Commons.Web.Bundling
{
    /// <summary>
    /// Provides a way to configure bundled resources for web applications based on JSON representation of
    /// a BundleConfiguration object.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IBundleConfigurator
    {
        /// <summary>
        /// Configures the bundled resources for the current web application.
        /// </summary>
        /// <param name="bundleConfigurationJson">JSON representation of a BundleConfiguration object.</param>
        void Configure(string bundleConfigurationJson);
    }
}