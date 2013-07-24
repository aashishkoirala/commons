/*******************************************************************************************************************************
 * AK.Commons.InitializationException
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
using System.Runtime.Serialization;
using AK.Commons.Exceptions;

#endregion

namespace AK.Commons
{
    /// <summary>
    /// Represents an exception during application environment initialization.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class InitializationException : ReasonedException<InitializationExceptionReason>
    {
        public InitializationException(InitializationExceptionReason reason) : base(reason) {}
        public InitializationException(InitializationExceptionReason reason, string message) : base(reason, message) {}
        public InitializationException(InitializationExceptionReason reason, Exception innerException) : base(reason, innerException) {}
        public InitializationException(InitializationExceptionReason reason, string message, Exception innerException) : base(reason, message, innerException) {}
        
        protected InitializationException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        protected override string GetReasonDescription(InitializationExceptionReason reason)
        {
            // TODO: Implement.

            return reason.ToString();
        }
    }
}