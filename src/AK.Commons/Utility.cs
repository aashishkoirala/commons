/*******************************************************************************************************************************
 * AK.Commons.Utility
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace AK.Commons
{
    /// <summary>
    /// A mixed bag of general purpose utility methods and extension methods.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class Utility
    {
        #region Enumerable Operations

        /// <summary>
        /// Performs the given action for each item in the enumerable. This one does a "for" loop
        /// instead of a "foreach" loop so you get the index as well as the value in the action.
        /// </summary>
        /// <typeparam name="TItem">Enumerable element type.</typeparam>
        /// <param name="target">Enumerable instance.</param>
        /// <param name="action">Action to execute.</param>
        public static void For<TItem>(this IEnumerable<TItem> target, Action<TItem, int> action)
        {
            var index = 0;
            foreach (var item in target)
            {
                action(item, index);
                index++;
            }
        }

        /// <summary>
        /// Performs the given action for each item in the enumerable. This one does a "foreach"
        /// loop so you get the item in the action.
        /// </summary>
        /// <typeparam name="TItem">Enumerable element type.</typeparam>
        /// <param name="target">Enumerable instance.</param>
        /// <param name="action">Action to execute.</param>
        public static void ForEach<TItem>(this IEnumerable<TItem> target, Action<TItem> action)
        {
            foreach (var item in target)
                action(item);
        }

        /// <summary>
        /// Performs the given action for each item in the dictionary. This one does a "foreach"
        /// loop so you get the key and value in the action.
        /// </summary>
        /// <typeparam name="TKey">Dictionary key type.</typeparam>
        /// <typeparam name="TValue">Dictionary value type.</typeparam>
        /// <param name="target">Dictionary instance.</param>
        /// <param name="action">Action to execute.</param>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> target, Action<TKey, TValue> action)
        {
            foreach (var item in target)
                action(item.Key, item.Value);
        }

        public static void AssignTo<T>(this IReadOnlyCollection<T> source, ICollection<T> target)
        {
            target.Clear();
            source.Copy(target);
        }

        public static void Copy<T>(this IReadOnlyCollection<T> source, ICollection<T> target)
        {
            foreach (var item in source)
                target.Add(item);
        }

        #endregion

        #region Object Serialization/Formatting

        /// <summary>
        /// Serializes the given object into a string representation as specified
        /// in the format.
        /// </summary>
        /// <param name="target">Object to serialize.</param>
        /// <param name="format">
        /// Format string. To reference the object's properties, use the format
        /// {PropertyName}.
        /// </param>
        /// <example>
        /// For an object with properties, say Index = 2 and Value = "Test Value"
        /// and format "Object is {Index} and {Value}", the result will be:
        /// "Object is 2 and Test Value".
        /// </example>
        /// <returns></returns>
        public static string ToFormattedString(this object target, string format)
        {
            var returnValue = format;

            foreach (var property in target.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanRead))
            {
                var propertyValue = property.GetValue(target);
                var propertyValueString = string.Format("{0}", propertyValue);
                var token = "{" + property.Name + "}";

                returnValue = returnValue.Replace(token, propertyValueString);
            }

            return returnValue;
        }

        #endregion

        #region Enum Operations

        /// <summary>
        /// Returns a friendly human readable description for the given Enum. The information is extracted from
        /// the EnumDescriptionAttribute applied to the Enum member that corresponds to this Enum's value. If
        /// the attribute is not applied, this method reverts to Enum.ToString().
        /// </summary>
        /// <typeparam name="TEnum">Enum type.</typeparam>
        /// <param name="enumObject">Enum instance.</param>
        /// <returns>Friendly human readable description.</returns>
        public static string Describe<TEnum>(this TEnum enumObject)
        {
            var enumType = typeof (TEnum);
            if (enumType == typeof (Enum)) enumType = enumObject.GetType();

            var memberName = Enum.GetName(enumType, enumObject);
            var fieldInfo = enumType.GetField(memberName);
            var descriptionAttribute = fieldInfo.GetCustomAttribute<EnumDescriptionAttribute>();
            return descriptionAttribute == null ? enumObject.ToString() : descriptionAttribute.Description;
        }

        #endregion

        #region Uri Operations

        /// <summary>
        /// Appends the given path to the given Uri.
        /// </summary>
        /// <param name="uri">Uri to append to.</param>
        /// <param name="pathToAppend">Path to append.</param>
        /// <returns>Resulting Uri after appending.</returns>
        public static Uri Append(this Uri uri, string pathToAppend)
        {
            var uriString = uri.ToString();
            if (!uriString.EndsWith("/")) uriString += "/";
            if (pathToAppend.StartsWith("/")) pathToAppend = pathToAppend.Substring(1);

            return new Uri(uriString + pathToAppend);
        }

        /// <summary>
        /// Extracts just the scheme and host portion of the given Uri.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <returns>Uri representing just the scheme and host.</returns>
        public static Uri GetSchemeAndHost(this Uri uri)
        {
            var scheme = uri.Scheme + ":";
            var host = uri.Host;

            if (scheme != "urn" && scheme != "mailto") scheme += "//";
            if (!uri.IsDefaultPort) host += ":" + uri.Port;

            return new Uri(scheme + host);
        }

        /// <summary>
        /// Removes the QueryString portion from the given Uri.
        /// </summary>
        /// <param name="uri">Uri to remove from.</param>
        /// <returns>Uri with QueryString removed.</returns>
        public static Uri RemoveQueryString(this Uri uri)
        {
            return new Uri(uri.GetSchemeAndHost(), uri.AbsolutePath);
        }

        /// <summary>
        /// Removes the scheme from the given Uri and returns the resulting string representation.
        /// </summary>
        /// <param name="uri">Uri to remove scheme from.</param>
        /// <returns>String representatino of Uri without scheme.</returns>
        public static string RemoveScheme(this Uri uri)
        {
            var host = uri.Host;
            if (!uri.IsDefaultPort) host += ":" + uri.Port;

            return host + uri.PathAndQuery;
        }

        #endregion
    }
}