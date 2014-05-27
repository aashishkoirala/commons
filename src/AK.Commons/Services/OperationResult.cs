/*******************************************************************************************************************************
 * AK.Commons.Services.OperationResult
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
using System.Runtime.Serialization;

#endregion

namespace AK.Commons.Services
{
    /// <summary>
    /// Encapsulates results from a service operation.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [DataContract]
    public class OperationResult
    {
        /// <summary>
        /// Was the operation successful?
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Error code, if any.
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Message, if any.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Key to refer to in case of error, if any.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Creates a new successful result instance.
        /// </summary>
        public OperationResult()
        {
            this.IsSuccess = true;
        }

        /// <summary>
        /// Creates a copy of the given result.
        /// </summary>
        /// <param name="result">Result to copy.</param>
        public OperationResult(OperationResult result)
        {
            this.IsSuccess = result.IsSuccess;
            this.ErrorCode = result.ErrorCode;
            this.Message = result.Message;
            this.Key = result.Key;
        }

        /// <summary>
        /// Creates an error result using the given information.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="key">Key.</param>
        /// <param name="message">Message.</param>
        public OperationResult(Enum errorCode, object key = null, string message = null)
        {
            this.IsSuccess = false;
            this.ErrorCode = errorCode.ToString();
            this.Key = key == null ? string.Empty : key.ToString();
            this.Message = string.Format("{0} {1}", errorCode.Describe(), message ?? string.Empty).Trim();
        }
    }

    /// <summary>
    /// Encapsulates results from a service operation.
    /// </summary>
    /// <typeparam name="TResult">Type of result returned.</typeparam>
    /// <author>Aashish Koirala</author>
    [DataContract]
    public class OperationResult<TResult> : OperationResult
    {
        /// <summary>
        /// Underlying result.
        /// </summary>
        [DataMember]
        public TResult Result { get; set; }

        /// <summary>
        /// Creates a successful result with the given value.
        /// </summary>
        /// <param name="result">Result to encapsulate.</param>
        public OperationResult(TResult result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Creates an error result using the given information.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="key">Key.</param>
        /// <param name="message">Message.</param>
        public OperationResult(Enum errorCode, object key = null, string message = null) :
            base(errorCode, key, message)
        {
        }

        /// <summary>
        /// Creates a copy of the given result.
        /// </summary>
        /// <param name="result">Result to copy.</param>
        public OperationResult(OperationResult result)
            : base(result)
        {
        }
    }
}