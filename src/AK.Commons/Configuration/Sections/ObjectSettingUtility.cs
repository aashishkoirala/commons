/*******************************************************************************************************************************
 * AK.Commons.Configuration.Sections
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace AK.Commons.Configuration.Sections
{
    internal static class ObjectSettingUtility
    {
        public static object GetObject(this SettingsConfigurationElement settingsConfigurationElement)
        {
            var type = Type.GetType(settingsConfigurationElement.Type);
            Debug.Assert(type != null);

            if (type.IsEnum) return Enum.Parse(type, settingsConfigurationElement.Value);
            if (type == typeof(DateTime)) return DateTime.Parse(settingsConfigurationElement.Value);

            return type.IsPrimitive || type == typeof(string) ? 
                Convert.ChangeType(settingsConfigurationElement.Value, type) : 
                GetObjectFromObjectSettingConfigurationElement(settingsConfigurationElement);
        }

        public static object GetObject(this StoreConfigurationElement storeConfigurationElement)
        {
            var type = Type.GetType(storeConfigurationElement.Type);
            Debug.Assert(type != null);

            return GetObjectFromObjectSettingConfigurationElement(storeConfigurationElement);
        }

        private static object GetObjectFromObjectSettingConfigurationElement(IObjectSettingConfigurationElement settingsConfigurationElement)
        {
            var type = Type.GetType(settingsConfigurationElement.Type);
            Debug.Assert(type != null);

            object value;
            if (settingsConfigurationElement.ConstructorParameters != null && settingsConfigurationElement.ConstructorParameters.Count > 0)
            {
                var paramHash = settingsConfigurationElement.ConstructorParameters.ObjectProperties;
                var constructorInfo = MatchConstructorByParameterNames(type, paramHash.Keys.ToList());
                Debug.Assert(constructorInfo != null);

                var constructorParams = constructorInfo.GetParameters();
                var constructorParamList =
                    (from parameterInfo in constructorParams
                     let paramValue = paramHash[parameterInfo.Name].Value
                     select Convert.ChangeType(paramValue, parameterInfo.ParameterType)).ToArray();

                value = Activator.CreateInstance(type, constructorParamList);
            }
            else value = Activator.CreateInstance(type);

            Debug.Assert(value != null);

            if (settingsConfigurationElement.ObjectProperties == null || settingsConfigurationElement.ObjectProperties.ObjectProperties.Count == 0)
                return value;

            foreach (var pair in settingsConfigurationElement.ObjectProperties.ObjectProperties)
            {
                var propertyInfo = type.GetProperty(pair.Key);
                Debug.Assert(propertyInfo != null);

                var propertyValue = pair.Value.Value;
                var propertyValueCast = Convert.ChangeType(propertyValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(value, propertyValueCast);
            }

            return value;
        }

        private static ConstructorInfo MatchConstructorByParameterNames(Type type, IList<string> parameterNames)
        {
            var constructors = type.GetConstructors();
            return (
                from constructor in constructors
                let parameters = constructor.GetParameters()
                where parameters.Length == parameterNames.Count
                where !parameters.Where((t, i) => t.Name != parameterNames[i]).Any()
                select constructor).FirstOrDefault();
        }
    }
}