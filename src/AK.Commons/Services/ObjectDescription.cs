/*******************************************************************************************************************************
 * AK.Commons.Services
 * 
 * THIS NAMESPACE IS UNDER DEVELOPMENT.
 * 
 * TODO: Build WCF library within AK.Common.Services.
 * 
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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AK.Commons.Services
{
    public class ObjectDescription
    {
        public string TypeFullName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyPath { get; set; }

        public List<PairDescription> Properties { get; set; }
        public List<PairDescription> ConstructorParameters { get; set; }

        public class PairDescription
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public T Instantiate<T>()
        {
            // TODO: Implement constructor using constructor parameters and properties.

            var type = Type.GetType(this.TypeFullName);
            Debug.Assert(type != null);

            return (T) Activator.CreateInstance(type);
        }
    }
}