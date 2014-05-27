/*******************************************************************************************************************************
 * AK.Commons.Composition.ComposerException
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

using AK.Commons.Exceptions;
using System;
using System.Runtime.Serialization;

#endregion

namespace AK.Commons.Composition
{
    /// <summary>
    /// Represents an exception during MEF composition or import resolution.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class ComposerException : ReasonedException<ComposerExceptionReason>
    {
        public ComposerException(ComposerExceptionReason reason) : base(reason) {}
        public ComposerException(ComposerExceptionReason reason, string message) : base(reason, message) {}
        public ComposerException(ComposerExceptionReason reason, Exception innerException) : base(reason, innerException) {}
        public ComposerException(ComposerExceptionReason reason, string message, Exception innerException) : base(reason, message, innerException) {}
        
        protected ComposerException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}