/*******************************************************************************************************************************
 * AK.Commons.Utility
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
    }
}