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

using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
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
        #region Config Key Constants

        private const string ConfigKeyLoginIssuerUrl = "ak.commons.security.loginissuerurl";

        private const string ConfigKeyApplyRestrictIpBehavior =
            "ak.commons.security.loginservicehostfactory.applyrestrictipbehavior";

        private const string ConfigKeyDisableAddressFilter =
            "ak.commons.security.loginservicehostfactory.disableaddressfilter";

        private const string ConfigKeySkipCertificateValidation =
            "ak.commons.security.loginservicehostfactory.skipcertificatevalidation";

        private const string ConfigKeyNegotiateServiceCredential =
            "ak.commons.security.loginservicehostfactory.negotiateservicecredential";

        private const string ConfigKeyEstablishSecurityContext =
            "ak.commons.security.loginservicehostfactory.establishsecuritycontext";

        #endregion

        private readonly IAppConfig config = AppEnvironment.Config;
        private readonly ICertificateProvider certificateProvider = AppEnvironment.CertificateProvider;

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var serviceHost = base.CreateServiceHost(constructorString, baseAddresses);

            serviceHost.Description.Behaviors.Remove<ServiceCredentials>();

            var applyRestrictIpBehavior = this.config.Get(ConfigKeyApplyRestrictIpBehavior, true);
            if (applyRestrictIpBehavior)
                serviceHost.Description.Behaviors.Add(this.GetRestrictIpServiceBehavior(baseAddresses));

            var disableAddressFilter = this.config.Get(ConfigKeyDisableAddressFilter, false);
            if (disableAddressFilter) DisableAddressFilter(serviceHost.Description);

            serviceHost.Description.Behaviors.Add(this.GetCertificateServiceBehavior());

            var negotiateServiceCredential = this.config.Get(ConfigKeyNegotiateServiceCredential, false);
            var establishSecurityContext = this.config.Get(ConfigKeyEstablishSecurityContext, true);

            var endpoints = this.CreateEndpoints(baseAddresses, negotiateServiceCredential, establishSecurityContext);
            foreach (var endpoint in endpoints) serviceHost.Description.Endpoints.Add(endpoint);

            AssignMetadataBehaviors(serviceHost.Description.Behaviors);

            return serviceHost;
        }

        private static void DisableAddressFilter(ServiceDescription description)
        {
            var serviceBehaviorAttribute = description.Behaviors.Find<ServiceBehaviorAttribute>();

            if (serviceBehaviorAttribute != null)
            {
                serviceBehaviorAttribute.AddressFilterMode = AddressFilterMode.Any;
                return;
            }

            description.Behaviors.Add(new ServiceBehaviorAttribute {AddressFilterMode = AddressFilterMode.Any});
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

            var skipCertificateValidation = this.config.Get(ConfigKeySkipCertificateValidation, false);
            if (!skipCertificateValidation) return credentials;

            credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            return credentials;
        }

        private IEnumerable<ServiceEndpoint> CreateEndpoints(
            IEnumerable<Uri> baseAddresses, bool negotiateServiceCredential, bool establishSecurityContext)
        {
            foreach (var baseAddress in baseAddresses)
            {
                var binding = new WSHttpBinding(SecurityMode.Message);
                binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                binding.Security.Message.NegotiateServiceCredential = negotiateServiceCredential;
                binding.Security.Message.EstablishSecurityContext = establishSecurityContext;

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