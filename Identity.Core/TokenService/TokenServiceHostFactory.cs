/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using Highway.Identity.Core.Repositories;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;

namespace Highway.Identity.Core.TokenService
{
    /// <summary>
    /// Abstracts away the details of the WS-Trust ServiceHost creation and configuration
    /// </summary>
    public class TokenServiceHostFactory : ServiceHostFactory
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly Func<TokenServiceConfiguration> _tokenServiceFactory;

        public TokenServiceHostFactory(IConfigurationRepository configurationRepository, Func<TokenServiceConfiguration> tokenServiceFactory) : base()
        {
            _configurationRepository = configurationRepository;
            _tokenServiceFactory = tokenServiceFactory;
        }

        /// <summary>
        /// Creates a service host to process WS-Trust 1.3 requests
        /// </summary>
        /// <param name="constructorString">The constructor string.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        /// <returns>A WS-Trust ServiceHost</returns>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var globalConfiguration = _configurationRepository.Configuration;
            var config = CreateSecurityTokenServiceConfiguration(constructorString);
            var host = new WSTrustServiceHost(config, baseAddresses);
            
            // add behavior for load balancing support
            host.Description.Behaviors.Add(new UseRequestHeadersForMetadataAddressBehavior());

            // modify address filter mode for load balancing
            var serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            serviceBehavior.AddressFilterMode = AddressFilterMode.Any;

            // add and configure a mixed mode security endpoint
            if (_configurationRepository.Endpoints.WSTrustMixed)
            {
                EndpointIdentity epi = null;
                if (_configurationRepository.Configuration.EnableStrongEpiForSsl)
                {
                    if (_configurationRepository.SslCertificate.Certificate == null)
                    {
                        throw new ServiceActivationException("No SSL certificate configured for strong endpoint identity.");
                    }

                    epi = EndpointIdentity.CreateX509CertificateIdentity(_configurationRepository.SslCertificate.Certificate);
                }

                if (globalConfiguration.EnableClientCertificates)
                {
                    var sep2 = host.AddServiceEndpoint(
                        typeof(IWSTrust13SyncContract),
                        new CertificateWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                        Endpoints.Paths.WSTrustMixedCertificate);

                    if (epi != null)
                    {
                        sep2.Address = new EndpointAddress(sep2.Address.Uri, epi);
                    }
                }

                var sep = host.AddServiceEndpoint(
                    typeof(IWSTrust13SyncContract),
                    new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                    Endpoints.Paths.WSTrustMixedUserName);

                if (epi != null)
                {
                    sep.Address = new EndpointAddress(sep.Address.Uri, epi);
                }
            }

            // add and configure a message security endpoint
            if (_configurationRepository.Endpoints.WSTrustMessage)
            {
                var credential = new ServiceCredentials();
                credential.ServiceCertificate.Certificate = _configurationRepository.SigningCertificate.Certificate;
                host.Description.Behaviors.Add(credential);

                if (globalConfiguration.EnableClientCertificates)
                {
                    host.AddServiceEndpoint(
                        typeof(IWSTrust13SyncContract),
                        new CertificateWSTrustBinding(SecurityMode.Message),
                        Endpoints.Paths.WSTrustMessageCertificate);
                }

                host.AddServiceEndpoint(
                    typeof(IWSTrust13SyncContract),
                    new UserNameWSTrustBinding(SecurityMode.Message),
                    Endpoints.Paths.WSTrustMessageUserName);
            }

            return host;
        }

        protected virtual SecurityTokenServiceConfiguration CreateSecurityTokenServiceConfiguration(string constructorString)
        {
            var type = Type.GetType(constructorString, true);
            if (type != typeof(TokenServiceConfiguration))
            {
                throw new InvalidOperationException("SecurityTokenServiceConfiguration");
            }

            return _tokenServiceFactory();
        }
    }
}