/*******************************************************************************************************************************
 * AK.Commons.Configuration.Sections
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

using System.Configuration;
using System.Diagnostics;

#endregion

namespace AK.Commons.Configuration.Sections
{
    /// <summary>
    /// This class is used internally to read configuration settings.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class ApplicationConfigurationElement : ConfigurationElement
    {
        private const string NameProperty = "name";
        private const string NamePropertyDefaultValue = "";
        private const string SettingsProperty = "settings";
        private const string SettingsAddItemName = "setting";
        private const string TokensProperty = "tokens";
        private const string TokensAddItemName = "token";

        [ConfigurationProperty(NameProperty, IsKey = true, DefaultValue = NamePropertyDefaultValue)]
        public string Name
        {
            get
            {
                var value = this[NameProperty] as string;
                Debug.Assert(value != null);
                return value;
            }
            set { this[NameProperty] = value; }
        }

        [ConfigurationProperty(SettingsProperty)]
        [ConfigurationCollection(typeof(SettingsConfigurationElementCollection), AddItemName = SettingsAddItemName)]
        public SettingsConfigurationElementCollection Settings
        {
            get 
            { 
                var value = this[SettingsProperty] as SettingsConfigurationElementCollection;
                Debug.Assert(value != null);
                return value;
            }
            set { this[SettingsProperty] = value; }
        }

        [ConfigurationProperty(TokensProperty)]
        [ConfigurationCollection(typeof(TokensConfigurationElementCollection), AddItemName = TokensAddItemName)]
        public TokensConfigurationElementCollection Tokens
        {
            get
            {
                var value = this[TokensProperty] as TokensConfigurationElementCollection;
                Debug.Assert(value != null);
                return value;
            }
            set { this[TokensProperty] = value; }
        }
    }
}