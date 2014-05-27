/*******************************************************************************************************************************
 * AK.Commons.Security.LoginServiceHostFactory
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

using AK.Commons.Configuration;
using AK.Commons.Services.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

#endregion

namespace AK.Commons.Security
{
    /// <summary>
    /// WCF ServiceHostFactory to use with WCF implementations of ILoginService. Since the service
    /// is supposed to be hosted publicly but should only be called by the STS, it is secured using
    /// an IP-restrict behavior and certificate.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class LoginServiceHostFactory : ServiceHostFactory
    {
        private const string ConfigKeyLoginIssuerUrl = "ak.commons.security.loginissuerurl";

        private readonly IAppConfig config = AppEnvironment.Config;
        private readonly ICertificateProvider certificateProvider = AppEnvironment.CertificateProvider;

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var serviceHost = base.CreateServiceHost(constructorString, baseAddresses);

            serviceHost.Description.Behaviors.Remove<ServiceCredentials>();

            serviceHost.Description.Behaviors.Add(this.GetRestrictIpServiceBehavior(baseAddresses));
            serviceHost.Description.Behaviors.Add(this.GetCertificateServiceBehavior());

            foreach (var endpoint in this.CreateEndpoints(baseAddresses))
                serviceHost.Description.Endpoints.Add(endpoint);

            AssignMetadataBehaviors(serviceHost.Description.Behaviors);

            return serviceHost;
        }

        private IServiceBehavior GetRestrictIpServiceBehavior(IEnumerable<Uri> baseAddresses)
        {
            var loginIsserUrl = this.config.Get<string>(ConfigKeyLoginIssuerUrl);

            var host = new Uri(loginIsserUrl).Host;
            var hosts = baseAddresses.Select(x => x.Host).Concat(new[] {host});

            var allowedIps = hosts
                .Select(Dns.GetHostAddresses)
                .SelectMany(x => x)
                .Select(x => x.ToString())
                .ToArray();

            return new RestrictIpServiceBehavior(allowedIps, true);
        }

        private IServiceBehavior GetCertificateServiceBehavior()
        {
            var credentials = new ServiceCredentials();
            credentials.ServiceCertificate.Certificate = this.certificateProvider.Certificate;

            return credentials;
        }

        private IEnumerable<ServiceEndpoint> CreateEndpoints(IEnumerable<Uri> baseAddresses)
        {
            foreach (var baseAddress in baseAddresses)
            {
                var binding = new WSHttpBinding(SecurityMode.Message);
                binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

                var identity = new X509CertificateEndpointIdentity(this.certificateProvider.Certificate);

                var address = new EndpointAddress(baseAddress, identity);

                var contract = ContractDescription.GetContract(typeof (ILoginService));
                var endpoint = new ServiceEndpoint(contract, binding, address);

                yield return endpoint;
            }
        }

        private static void AssignMetadataBehaviors(ICollection<IServiceBehavior> behaviors)
        {
            var metadataBehaviors = behaviors.OfType<ServiceMetadataBehavior>().ToArray();

            if (!metadataBehaviors.Any())
                behaviors.Add(new ServiceMetadataBehavior {HttpGetEnabled = true});

            foreach (var metadataBehavior in metadataBehaviors)
                metadataBehavior.HttpGetEnabled = true;
        }
    }
}