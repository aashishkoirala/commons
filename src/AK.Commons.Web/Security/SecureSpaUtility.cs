﻿/*******************************************************************************************************************************
 * AK.Commons.Web.Security.SecureSpaUtility
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

using AK.Commons.Security;
using System;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace AK.Commons.Web.Security
{
    /// <summary>
    /// Utility methods related to secure SPAs.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class SecureSpaUtility
    {
        private const string ConfigKeyIssuer = "ak.commons.security.loginissuerurl";

        private static bool hasAddToAllowedAudienceBeenCalled;
        private static readonly object hasAddToAllowedAudienceBeenCalledLock = new object();

        /// <summary>
        /// Enables secure SPA routes.
        /// </summary>
        /// <param name="routes">Route collection.</param>
        /// <param name="mainPageHtmlPath">Path to main HTML page of SPA.</param>
        /// <param name="logoutAction">Stuff to do on logout, if any.</param>
        public static void MapSecureSpaRoutes(this RouteCollection routes, string mainPageHtmlPath, Action logoutAction)
        {
            SecureSpaController.MainPagePath = mainPageHtmlPath;
            SecureSpaController.LogoutAction = logoutAction;

            routes.MapRoute("Login", "login", new {controller = "SecureSpa", action = "Login"});
            routes.MapRoute("Logout", "logout", new {controller = "SecureSpa", action = "Logout"});
            routes.MapRoute("Error", "error", new {controller = "SecureSpa", action = "Error"});
            routes.MapRoute("Default", "{controller}/{action}", new {controller = "SecureSpa", action = "Index"});
        }

        /// <summary>
        /// Configures WIF-based secure login for the SPA.
        /// </summary>
        /// <param name="application">Web application.</param>
        /// <param name="secureLoginInfo">Object with secure login application properties.</param>
        public static void ConfigureSecureLogin(this HttpApplication application, SecureLoginInfo secureLoginInfo)
        {
            SanitizeSecureLoginInfo(secureLoginInfo);

            FederatedAuthentication.FederationConfigurationCreated += (s, args) =>
                {
                    args.FederationConfiguration.WsFederationConfiguration.Realm = secureLoginInfo.Realm;
                    args.FederationConfiguration.WsFederationConfiguration.Issuer = secureLoginInfo.Issuer;
                    args.FederationConfiguration.WsFederationConfiguration.RequireHttps = secureLoginInfo.RequireSsl;

                    args.FederationConfiguration.AssignSecurityTokenResolver(secureLoginInfo.Certificate);
                    args.FederationConfiguration.CookieHandler = new ChunkedCookieHandler
                        {
                            RequireSsl = secureLoginInfo.RequireSsl
                        };

                    if (!string.IsNullOrWhiteSpace(secureLoginInfo.AllowedAudienceUriList))
                    {
                        var urls = secureLoginInfo.AllowedAudienceUriList
                                                  .Split(';')
                                                  .Select(x => new Uri(x));

                        foreach (var url in urls) AddToAllowedAudience(args.FederationConfiguration, url);
                    }

                    if (secureLoginInfo.FurtherAction != null)
                        secureLoginInfo.FurtherAction(args.FederationConfiguration);
                };
        }

        /// <summary>
        /// Adds this application's URI to the list of WIF's allowed audiences.
        /// </summary>
        /// <param name="application">Web application.</param>
        public static void AddToAllowedAudience(this HttpApplication application)
        {
            if (hasAddToAllowedAudienceBeenCalled) return;

            lock (hasAddToAllowedAudienceBeenCalledLock)
            {
                if (hasAddToAllowedAudienceBeenCalled) return;

                var url = application.Request.Url.GetSchemeAndHost();
                AddToAllowedAudience(FederatedAuthentication.FederationConfiguration, url);

                hasAddToAllowedAudienceBeenCalled = true;
            }
        }

        private static void AddToAllowedAudience(FederationConfiguration federationConfiguration, Uri url)
        {
            var identityConfiguration = federationConfiguration.IdentityConfiguration;

            identityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(url);

            var urlString = url.ToString();
            if (urlString.EndsWith("/")) urlString = urlString.TrimEnd('/');
            else urlString += "/";

            url = new Uri(urlString);
            identityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(url);
        }

        private static void SanitizeSecureLoginInfo(SecureLoginInfo secureLoginInfo)
        {
            if (secureLoginInfo.Certificate == null)
                secureLoginInfo.Certificate = AppEnvironment.CertificateProvider.Certificate;

            if (secureLoginInfo.Issuer == null)
                secureLoginInfo.Issuer = AppEnvironment.Config.Get<string>(ConfigKeyIssuer);
        }
    }
}