/*******************************************************************************************************************************
 * AK.Commons.Configuration.AppConfigException
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

namespace AK.Commons.Configuration
{
    /// <summary>
    /// Represents an application configuration related exception.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class AppConfigException : ReasonedException<AppConfigExceptionReason>
    {
        public AppConfigException(AppConfigExceptionReason reason) : base(reason) { }
        public AppConfigException(AppConfigExceptionReason reason, string message) : base(reason, message) {}
        public AppConfigException(AppConfigExceptionReason reason, Exception innerException) : base(reason, innerException) {}
        public AppConfigException(AppConfigExceptionReason reason, string message, Exception innerException) : base(reason, message, innerException) {}

        protected AppConfigException(SerializationInfo info, StreamingContext context) : base(info, context) {}        
    }
}