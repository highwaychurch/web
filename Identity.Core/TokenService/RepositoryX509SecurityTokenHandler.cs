﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using Highway.Identity.Core.Repositories;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Tokens;

namespace Highway.Identity.Core.TokenService
{
    public class RepositoryX509SecurityTokenHandler : X509SecurityTokenHandler
    {
        public static Func<IUserRepository> UserRepositoryFactoryMethod { get; set; } 

        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            Tracing.Tracing.Information("Beginning client certificate token validation and authentication for SOAP");
            
            // call base class implementation for validation and claims generation 
            var identity = base.ValidateToken(token).First();

            // retrieve thumbprint
            var clientCert = ((X509SecurityToken)token).Certificate;
            Tracing.Tracing.Information(String.Format("Client certificate thumbprint: {0}", clientCert.Thumbprint));

            // check if mapped user exists
            string userName;
            var userRepository = UserRepositoryFactoryMethod();
            if (!userRepository.ValidateUser(clientCert, out userName))
            {
                var message = String.Format("No mapped user exists for thumbprint {0}", clientCert.Thumbprint);
                Tracing.Tracing.Error(message);
                throw new SecurityTokenValidationException(message);
            }

            Tracing.Tracing.Information(String.Format("Mapped user found: {0}", userName));

            // retrieve issuer name
            var issuer = identity.Claims.First().Issuer;
            Tracing.Tracing.Information(String.Format("Certificate issuer: {0}", issuer));

            // create new ClaimsIdentity for the STS issuance logic
            var claims = new List<Claim>
            {
                new Claim(WSIdentityConstants.ClaimTypes.Name, userName),
                new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.X509)
            };

            var id = new ClaimsIdentity(claims);
            return new ClaimsIdentityCollection(new IClaimsIdentity[] { id });
        }
    }
}