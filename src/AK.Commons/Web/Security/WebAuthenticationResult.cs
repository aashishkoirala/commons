/*******************************************************************************************************************************
 * AK.Commons.Web.Security.WebAuthenticationResult
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

#endregion

namespace AK.Commons.Web.Security
{
    /// <summary>
    /// Represents the result of a web authentication operation.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class WebAuthenticationResult
    {
        public WebAuthenticationResult()
        {
            this.UserAttributes = new Dictionary<string, object>();
        }

        /// <summary>
        /// Result type.
        /// </summary>
        public WebAuthenticationResultType ResultType { get; set; }

        /// <summary>
        /// User name, if authentication was successful.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Exception, if the result was an error.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Error message, if any - may be populated for denial as well as error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Miscellaneous user attributes that may be populated by the authentication provider.
        /// </summary>
        public IDictionary<string, object> UserAttributes { get; private set; }
    }
}