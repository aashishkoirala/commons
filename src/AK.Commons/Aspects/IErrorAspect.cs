/*******************************************************************************************************************************
 * AK.Commons.Aspects.IErrorAspect
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
using System.Reflection;

#endregion

namespace AK.Commons.Aspects
{
    /// <summary>
    /// Interface that an attribute must implement to be an error aspect.
    /// An error aspect is executed when an exception is caught within the operation.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IErrorAspect : IAspect
    {
        /// <summary>
        /// Executes the error aspect.
        /// </summary>
        /// <param name="memberInfo">
        /// MemberInfo representing the current method or property. In the case of a property,
        /// this will represent the inner get/set method.
        /// </param>
        /// <param name="parameters">
        /// Dictionary of parameter values passed to this operation, keyed by parameter names.
        /// </param>
        /// <param name="ex">
        /// Exception object that has been caught within the operation.
        /// This can be modified by the aspect.
        /// </param>
        /// <param name="returnValue">
        /// The return value of the operation, this is NULL if the method returns void,
        /// or if we're in a property set block. This can also be modified by the aspect.
        /// </param>
        /// <returns>
        /// Return TRUE to rethrow the caught exception. Return FALSE to not rethrow it.
        /// </returns>
        bool Execute(MemberInfo memberInfo, IDictionary<string, object> parameters, ref Exception ex,
                     ref object returnValue);
    }
}