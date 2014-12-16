/*******************************************************************************************************************************
 * AK.Commons.Web.Security.SecureLoginInfo
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
using System.IdentityModel.Services.Configuration;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace AK.Commons.Web.Security
{
    /// <summary>
    /// Contains information on configuring secure login for SPAs.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class SecureLoginInfo
    {
        /// <summary>
        /// The certificate to use for communication between the STS and the RP. If not set, the configured certificate
        /// provider is used.
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// The realm for the relying party. Must be set.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// The URL for the issuer (STS). If not set, value is retrieved from configuration.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Whether SSL is required.
        /// </summary>
        public bool RequireSsl { get; set; }

        /// <summary>
        /// Semicolon-separated list of audience URIs to be added to the allowed list.
        /// </summary>
        public string AllowedAudienceUriList { get; set; }

        /// <summary>
        /// Any other action to take as part of configuration of secure login.
        /// </summary>
        public Action<FederationConfiguration> FurtherAction { get; set; }
    }
}