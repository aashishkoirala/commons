/*******************************************************************************************************************************
 * AK.Commons.Web.Security.SecureSpaController
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Web Library (AKCWL).
 *  
 * AKCWL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCWL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCWL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.IdentityModel.Services;
using System.Web.Mvc;

#endregion

namespace AK.Commons.Web.Security
{
    /// <summary>
    /// General application controller for secure single-page applications (SPAs). Consists of
    /// a login, logout, default (index) and error actions. Call RouteTable.Routes.SecureSpaRoutes() to enable.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class SecureSpaController : Controller
    {
        private const string ConfigKeyLoginIssuerUrl = "ak.commons.security.loginissuerurl";

        /// <summary>
        /// Path to the main HTML page of the SPA.
        /// </summary>
        public static string MainPagePath { get; set; }

        /// <summary>
        /// Stuff to do on logout, if any.
        /// </summary>
        public static Action LogoutAction { get; set; }

        public ActionResult Index()
        {
            if (!this.User.Identity.IsAuthenticated) return this.RedirectToRoute("Login");

            var mainHtmlPath = this.Server.MapPath(MainPagePath);
            var mainHtmlContent = System.IO.File.ReadAllText(mainHtmlPath);

            return this.Content(mainHtmlContent, "text/html");
        }

        public ActionResult Error()
        {
            var issuerUrl = AppEnvironment.Config.Get<string>(ConfigKeyLoginIssuerUrl);
            var errorUrl = new Uri(issuerUrl).Append("Error").ToString();

            return this.Redirect(errorUrl);
        }

        public ActionResult Login()
        {
            return this.RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            FederatedAuthentication.WSFederationAuthenticationModule.SignOut(false);

            var authenticationModule = FederatedAuthentication.WSFederationAuthenticationModule;
            var url = WSFederationAuthenticationModule.GetFederationPassiveSignOutUrl(
                authenticationModule.Issuer, authenticationModule.Realm, null);

            if (LogoutAction != null) LogoutAction();

            return this.Redirect(url);
        }
    }
}