/*******************************************************************************************************************************
 * AK.Commons.Aspects.GeneratedTypeCache
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
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

#endregion

namespace AK.Commons.Aspects
{
    /// <summary>
    /// Provides thread-safe caching for aspect-wrapped generated types.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal static class GeneratedTypeCache
    {
        private static readonly IDictionary<Tuple<Type, Type>, Type> implementationMap =
            new Dictionary<Tuple<Type, Type>, Type>();

        private static readonly object implementationMapLock = new object();

        public static Type Get(Type contractType, Type implementationType, Func<Type> generator)
        {
            var keyTuple = new Tuple<Type, Type>(contractType, implementationType);

            Type generatedType;
            if (implementationMap.TryGetValue(keyTuple, out generatedType))
                return generatedType;

            generatedType = generator();

            lock (implementationMapLock)
            {
                if (!implementationMap.ContainsKey(keyTuple)) implementationMap[keyTuple] = generatedType;
                else generatedType = implementationMap[keyTuple];
            }

            return generatedType;
        }
    }
}