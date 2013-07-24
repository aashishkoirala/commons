/*******************************************************************************************************************************
 * AK.Commons.Configuration.IAppConfig
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

namespace AK.Commons.Configuration
{
    /// <summary>
    /// Interface that provides configuration services to the application. You should not directly implement
    /// this interface or register another MEF export for this. The application environment expects only
    /// one implementation for this which it provides. You can implement your own configuration store
    /// by implementing <see cref="IConfigStore"/> and use that though.
    /// 
    /// In order to use the system implementation of this, you need to:
    /// 1) Make sure you've called AppEnvironment.Initialize().
    /// 2) Either:
    ///    a) Create a MEF import against IAppConfig (preferably), or:
    ///    b) Use AppEnvironment.Config.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IAppConfig
    {
        /// <summary>
        /// Gets the value of the given configuration setting.
        /// </summary>
        /// <typeparam name="TItem">Setting type.</typeparam>
        /// <param name="key">Setting name or key.</param>
        /// <returns>Setting value.</returns>
        /// <exception cref="AppConfigException">If key is not found or is of the wrong type.</exception>
        TItem Get<TItem>(string key);

        /// <summary>
        /// Gets the value of the given configuration setting, or a default value if not found.
        /// </summary>
        /// <typeparam name="TItem">Setting type.</typeparam>
        /// <param name="key">Setting name or key.</param>
        /// <param name="defaultValue">The default value to use if the setting is not found.</param>
        /// <returns>Setting value.</returns>
        /// <exception cref="AppConfigException">If key is of the wrong type.</exception>
        TItem Get<TItem>(string key, TItem defaultValue);

        /// <summary>
        /// Tries to get the value of the given configuration setting.
        /// </summary>
        /// <typeparam name="TItem">Setting type.</typeparam>
        /// <param name="key">Setting name or key.</param>
        /// <param name="value">Out parameter to receive the value in.</param>
        /// <returns>Whether the value was found and retrieved.</returns>
        bool TryGet<TItem>(string key, out TItem value);
    }
}