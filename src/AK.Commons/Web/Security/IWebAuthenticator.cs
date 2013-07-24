/*******************************************************************************************************************************
 * AK.Commons.Web.Security.IWebAuthenticator
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
using System.Web.Mvc;

#endregion

namespace AK.Commons.Web.Security
{
    /// <summary>
    /// Performs authentication using an SSO provider.
    /// </summary>
    public interface IWebAuthenticator
    {
        /// <summary>
        /// Performs authentication using an SSO provider.
        /// </summary>
        /// <param name="authorizationContext">AuthorizationContext instance obtained from inside an authorization filter.</param>
        /// <param name="authenticationCallback">Callback to handle different authentication outcomes.</param>
        /// <param name="alreadyLoggedInCallback">Callback to handle the case where the user is already logged in.</param>
        void Authenticate(
            AuthorizationContext authorizationContext,
            Action<WebAuthenticationResult> authenticationCallback,
            Action alreadyLoggedInCallback);
    }
}