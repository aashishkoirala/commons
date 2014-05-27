/*******************************************************************************************************************************
 * AK.Commons.Providers.Security.PfxFileCertificateProvider
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
using AK.Commons.Security;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;

#endregion

namespace AK.Commons.Providers.Security
{
    /// <summary>
    /// Implementation of ICertificateProvider that provides X.509 certificates from a PFX file and password as provided
    /// in configuration.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (ICertificateProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ProviderMetadata("PfxFile")]
    public class PfxFileCertificateProvider : ICertificateProvider
    {
        #region Constants/Fields

        private const string ConfigPathKey = "ak.commons.security.certificateprovider.PfxFile.path";
        private const string ConfigPasswordKey = "ak.commons.security.certificateprovider.PfxFile.password";

        private readonly string path;
        private readonly string password;

        private X509Certificate2 certificate;

        #endregion

        [ImportingConstructor]
        public PfxFileCertificateProvider([Import] IAppConfig config)
        {
            this.path = config.Get<string>(ConfigPathKey);
            this.password = config.Get(ConfigPasswordKey, string.Empty);
        }

        public X509Certificate2 Certificate
        {
            get
            {
                this.certificate = this.certificate ?? this.LoadCertificate();
                return this.certificate;
            }
        }

        private X509Certificate2 LoadCertificate()
        {
            var certificateData = this.GetCertificateData();
            return string.IsNullOrWhiteSpace(this.password)
                       ? new X509Certificate2(certificateData)
                       : new X509Certificate2(certificateData, this.password);
        }

        private byte[] GetCertificateData()
        {
            if (File.Exists(this.path)) return File.ReadAllBytes(this.path);

            HttpContext httpContext;
            if ((this.path.StartsWith("/") || this.path.StartsWith("~/")) && (httpContext = HttpContext.Current) != null)
            {
                var filePath = httpContext.Server.MapPath(this.path);
                return File.ReadAllBytes(filePath);
            }

            if (this.path.StartsWith("http://") || this.path.StartsWith("https://"))
            {
                using (var webClient = new WebClient())
                    return webClient.DownloadData(this.path);
            }

            throw new Exception("Unrecognized certificate path.");
        }
    }
}