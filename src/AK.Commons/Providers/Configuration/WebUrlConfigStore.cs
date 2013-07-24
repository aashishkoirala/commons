/*******************************************************************************************************************************
 * AK.Commons.Providers.Configuration.WebUrlConfigStore
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

using System.Net;
using AK.Commons.Configuration;
using System.IO;

#endregion

namespace AK.Commons.Providers.Configuration
{
    /// <summary>
    /// Configuration store based on a web URL.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class WebUrlConfigStore : IConfigStore
    {
        /// <summary>
        /// The URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Whether to authenticate.
        /// </summary>
        public bool Authenticate { get; set; }

        /// <summary>
        /// Whether to use default credentials if authentication is turned on.
        /// </summary>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// User name to use if authentication is turned on.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password to use if authentication is turned on.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Domain to use if authentication is turned on.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets the configuration XML.
        /// </summary>
        /// <returns>Configuration XML</returns>
        public string GetConfigurationXml()
        {
            using (var webClient = new WebClient())
            {
                if (this.Authenticate)
                {
                    if (this.UseDefaultCredentials)
                        webClient.UseDefaultCredentials = true;

                    else if (string.IsNullOrWhiteSpace(this.UserName))
                        webClient.Credentials = CredentialCache.DefaultNetworkCredentials;

                    else if (string.IsNullOrWhiteSpace(this.Domain))
                        webClient.Credentials = new NetworkCredential(this.UserName, this.Password);

                    else webClient.Credentials = new NetworkCredential(
                        this.UserName, this.Password, this.Domain);
                }

                return webClient.DownloadString(this.Url);
            }            
        }
    }
}