/*******************************************************************************************************************************
 * AK.Commons.DataAccess.DataAccessException
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

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Represents a data access related exception.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class DataAccessException : ReasonedException<DataAccessExceptionReason>
    {
        public DataAccessException(DataAccessExceptionReason reason) : base(reason) { }
        public DataAccessException(DataAccessExceptionReason reason, string message) : base(reason, message) {}
        public DataAccessException(DataAccessExceptionReason reason, Exception innerException) : base(reason, innerException) {}
        public DataAccessException(DataAccessExceptionReason reason, string message, Exception innerException) : base(reason, message, innerException) {}

        protected DataAccessException(SerializationInfo info, StreamingContext context) : base(info, context) {}        
    }
}