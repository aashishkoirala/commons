/*******************************************************************************************************************************
 * AK.Commons.Exceptions.GeneralException
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

#endregion

namespace AK.Commons.Exceptions
{
    /// <summary>
    /// Represents a generic reasoned exception.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class GeneralException : ReasonedException<GeneralExceptionReason>
    {
        private const string GeneralErrorMessage = "An error has occurred.";

        public GeneralException() : base(GeneralExceptionReason.General) {}
        public GeneralException(string message) : base(GeneralExceptionReason.General, message) { }
        public GeneralException(Exception innerException) : base(GeneralExceptionReason.General, innerException) { }
        public GeneralException(string message, Exception innerException) : base(GeneralExceptionReason.General, message, innerException) { }
        protected GeneralException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        protected override string GetReasonDescription(GeneralExceptionReason reason)
        {
            return GeneralErrorMessage;
        }
    }
}