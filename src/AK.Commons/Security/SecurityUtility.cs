/*******************************************************************************************************************************
 * AK.Commons.Security.SecurityUtility
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
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

#endregion

namespace AK.Commons.Security
{
    /// <summary>
    /// Utility methods related to security.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class SecurityUtility
    {
        /// <summary>
        /// Given a FederationConfiguration instance, creates and assigns a session security token
        /// handler that uses Deflate and RSA for cookie transforms based on the provided
        /// certificate. Also adds the certificate issuer as a trusted issuer to the registry
        /// configured within the given FederationConfiguration instance.
        /// </summary>
        /// <param name="federationConfiguration">FederationConfiguration instance from WIF.</param>
        /// <param name="certificate">X.509 certificate.</param>
        public static void AssignSecurityTokenResolver(
            this FederationConfiguration federationConfiguration, X509Certificate2 certificate)
        {
            var identityConfiguration = federationConfiguration.IdentityConfiguration;

            var cookieTransforms = new ReadOnlyCollection<CookieTransform>(new CookieTransform[]
                {
                    new DeflateCookieTransform(),
                    new RsaEncryptionCookieTransform(certificate),
                    new RsaSignatureCookieTransform(certificate)
                });

            var securityTokenHandler = new SessionSecurityTokenHandler(cookieTransforms);

            identityConfiguration.SecurityTokenHandlers.AddOrReplace(securityTokenHandler);
            federationConfiguration.ServiceCertificate = certificate;

            var configurationBasedIssuerNameRegistry =
                identityConfiguration.IssuerNameRegistry as ConfigurationBasedIssuerNameRegistry;

            if (configurationBasedIssuerNameRegistry == null) return;
            if (certificate.Thumbprint == null) return;

            configurationBasedIssuerNameRegistry.AddTrustedIssuer(certificate.Thumbprint, certificate.IssuerName.Name);
        }

        /// <summary>
        /// Extracts the user ID from the given principal if it is a claims based principal.
        /// Uses the "sid" claim value as the user ID.
        /// </summary>
        /// <typeparam name="T">Type of user ID.</typeparam>
        /// <param name="principal">Claims principal.</param>
        /// <returns>
        /// Perhaps of T encompassing the user ID value, or an empty one if the
        /// principal is not authenticated or not a claims principal.
        /// </returns>
        public static Perhaps<T> GetUserId<T>(this IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated) return Perhaps<T>.NotThere;

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null) return Perhaps<T>.NotThere;

            var claimsIdentity = claimsPrincipal.Identities.Single();
            var sidClaim = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Sid);
            if (sidClaim == null) return Perhaps<T>.NotThere;

            return (T) Convert.ChangeType(sidClaim.Value, typeof (T));
        }
    }
}