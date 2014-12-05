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
    public class StoreConfigurationElement : ConfigurationElement, IObjectSettingConfigurationElement
    {
        private const string TypeProperty = "type";
        private const string PropertiesProperty = "properties";
        private const string PropertiesAddItemName = "property";
        private const string ConstructorParametersProperty = "constructorParameters";
        private const string ConstructorParametersAddItemName = "param";

        [ConfigurationProperty(TypeProperty)]
        public string Type
        {
            get
            {
                var value = this[TypeProperty] as string;
                Debug.Assert(value != null);
                return value;
            }
            set { this[TypeProperty] = value; }
        }

        [ConfigurationProperty(PropertiesProperty, IsRequired = false)]
        [ConfigurationCollection(typeof(PropertyConfigurationElementCollection), AddItemName = PropertiesAddItemName)]
        public PropertyConfigurationElementCollection ObjectProperties
        {
            get 
            { 
                var value = this[PropertiesProperty] as PropertyConfigurationElementCollection;
                Debug.Assert(value != null);
                return value;
            }
            set { this[PropertiesProperty] = value; }
        }

        [ConfigurationProperty(ConstructorParametersProperty, IsRequired = false)]
        [ConfigurationCollection(typeof(PropertyConfigurationElementCollection), AddItemName = ConstructorParametersAddItemName)]
        public PropertyConfigurationElementCollection ConstructorParameters
        {
            get
            {
                var value = this[ConstructorParametersProperty] as PropertyConfigurationElementCollection;
                Debug.Assert(value != null);
                return value;
            }
            set { this[ConstructorParametersProperty] = value; }
        }
    }
}