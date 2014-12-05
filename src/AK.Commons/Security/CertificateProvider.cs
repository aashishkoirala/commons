/*******************************************************************************************************************************
 * AK.Commons.Security.CertificateProvider
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

using AK.Commons.Composition;
using AK.Commons.Configuration;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace AK.Commons.Security
{
    /// <summary>
    /// Represents something that can give you a X.509 certificate.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface ICertificateProvider
    {
        /// <summary>
        /// X.509 certificate.
        /// </summary>
        X509Certificate2 Certificate { get; }
    }

    /// <summary>
    /// Creates ICertificateProvider instances based on provided IAppConfig.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal static class CertificateProviderFactory
    {
        private const string ConfigProviderNameKey = "ak.commons.security.certificateprovider.provider";

        public static ICertificateProvider Create(IComposer composer, IAppConfig appConfig)
        {
            var provider = appConfig.Get<string>(ConfigProviderNameKey);
            return composer.Resolve<ICertificateProvider, IProviderMetadata>(metadata => metadata.Provider == provider);
        }
    }
}