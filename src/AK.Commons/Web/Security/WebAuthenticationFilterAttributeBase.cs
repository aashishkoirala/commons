/*******************************************************************************************************************************
 * AK.Commons.Web.Security.WebAuthenticationFilterAttributeBase
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

using System.Web.Mvc;

namespace AK.Commons.Web.Security
{
    /// <summary>
    /// Base class for IAuthorizationFilter implementation that uses the configured IWebAuthenticator
    /// to perform authentication using the given SSO provider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class WebAuthenticationFilterAttributeBase : ActionFilterAttribute, IAuthorizationFilter
    {
        private readonly IWebAuthenticator webAuthenticator;

        public WebAuthenticationFilterAttributeBase()
        {
            this.webAuthenticator = WebAuthenticatorFactory.Create(AppEnvironment.Composer, AppEnvironment.Config);
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            this.webAuthenticator.Authenticate(filterContext, result =>
            {
                switch (result.ResultType)
                {
                    case WebAuthenticationResultType.Success:
                        this.OnSuccess(filterContext, result);
                        break;

                    case WebAuthenticationResultType.Denied:
                        this.OnDenied(filterContext, result);
                        break;

                    case WebAuthenticationResultType.Error:
                        this.OnError(filterContext, result);
                        break;
                }
            }, () => this.OnAlreadyLoggedIn(filterContext));
        }

        /// <summary>
        /// Override this to provide logic to be executed if the user is already logged in.
        /// </summary>
        /// <param name="filterContext"></param>
        protected virtual void OnAlreadyLoggedIn(AuthorizationContext filterContext) {}

        /// <summary>
        /// Override this to provide logic to be executed if authentication is successful.
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="authenticationResult"></param>
        protected virtual void OnSuccess(AuthorizationContext filterContext, WebAuthenticationResult authenticationResult) {}

        /// <summary>
        /// Override this to provide logic to be executed if the user is denied entry.
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="authenticationResult"></param>
        protected virtual void OnDenied(AuthorizationContext filterContext, WebAuthenticationResult authenticationResult) {}

        /// <summary>
        /// Override this to provide logic to be executed if there is an error during the authentication process.
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="authenticationResult"></param>
        protected virtual void OnError(AuthorizationContext filterContext, WebAuthenticationResult authenticationResult) {}
    }
}