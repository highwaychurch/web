/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.IdentityModel.Selectors;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;

namespace Highway.Identity.Core.TokenService
{
    /// <summary>
    /// Configuration information for the token service
    /// </summary>
    public class TokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        private static readonly object _syncRoot = new object();
        private static Lazy<TokenServiceConfiguration> _configuration = new Lazy<TokenServiceConfiguration>();

        readonly IConfigurationRepository _configurationRepository;

        public GlobalConfigurationModel GlobalConfiguration { get; protected set; }

        public TokenServiceConfiguration(IConfigurationRepository configurationRepository) : base()
        {
            _configurationRepository = configurationRepository;

            Tracing.Tracing.Information("Configuring token service");
            GlobalConfiguration = _configurationRepository.Configuration;

            SecurityTokenService = typeof(TokenService);
            DefaultTokenLifetime = TimeSpan.FromHours(GlobalConfiguration.DefaultTokenLifetime);
            MaximumTokenLifetime = TimeSpan.FromDays(GlobalConfiguration.MaximumTokenLifetime);
            DefaultTokenType = GlobalConfiguration.DefaultTokenType;

            TokenIssuerName = GlobalConfiguration.IssuerUri;
            SigningCredentials = new X509SigningCredentials(_configurationRepository.SigningCertificate.Certificate);

            if (GlobalConfiguration.EnableDelegation)
            {
                Tracing.Tracing.Information("Configuring identity delegation support");

                try
                {
                    var actAsRegistry = new ConfigurationBasedIssuerNameRegistry();
                    actAsRegistry.AddTrustedIssuer(_configurationRepository.SigningCertificate.Certificate.Thumbprint, GlobalConfiguration.IssuerUri);

                    var actAsHandlers = SecurityTokenHandlerCollectionManager["ActAs"];
                    actAsHandlers.Configuration.IssuerNameRegistry = actAsRegistry;
                    actAsHandlers.Configuration.AudienceRestriction.AudienceMode = AudienceUriMode.Never;
                }
                catch (Exception ex)
                {
                    Tracing.Tracing.Error("Error configuring identity delegation");
                    Tracing.Tracing.Error(ex.ToString());
                    throw;
                }
            }
        }

        public static TokenServiceConfiguration Current
        {
            get
            {
                return _configuration.Value;
            }
        }
    }
}