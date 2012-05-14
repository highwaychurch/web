﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityModel.Claims;
using Thinktecture.IdentityModel.Extensions;

namespace Highway.Identity.Core.TokenService
{
    /// <summary>
    /// This class contains the token issuance logic
    /// </summary>
    public class TokenService : SecurityTokenService
    {
        readonly IUserRepository _userRepository;

        public TokenService(SecurityTokenServiceConfiguration configuration, IUserRepository userRepository)
            : base(configuration)
        {
            _userRepository = userRepository;
        }

        protected GlobalConfigurationModel GlobalConfiguration
        {
            get
            {
                var config = SecurityTokenServiceConfiguration as TokenServiceConfiguration;
                return config.GlobalConfiguration;
            }
        }

        /// <summary>
        /// Analyzes the token request
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="request">The request.</param>
        /// <returns>A PolicyScope that describes the relying party and policy options</returns>
        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken rst)
        {
            if (rst.AppliesTo == null)
            {
                Tracing.Tracing.Error(string.Format("token request from {0} - but no realm specified.",
                    principal.Identity.Name));

                throw new MissingAppliesToException();
            }

            Tracing.Tracing.Information(string.Format("Starting token request from {0} for {1}",
                principal.Identity.Name,
                rst.AppliesTo.Uri.AbsoluteUri));

            Tracing.Tracing.Information("Authentication method: " + principal.Identities.First().GetClaimValue(ClaimTypes.AuthenticationMethod));

            // analyze request
            var request = new Request(GlobalConfiguration);
            var details = request.Analyze(rst, principal);

            // validate against policy
            request.Validate(details);

            // create scope
            var scope = new RequestDetailsScope(
                details, 
                SecurityTokenServiceConfiguration.SigningCredentials, 
                GlobalConfiguration.RequireEncryption);

            return scope;
        }

        /// <summary>
        /// Produces the output identity that gets transformed into a token
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="request">The request.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>An IClaimsIdentity describing the subject</returns>
        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            var requestDetails = (scope as RequestDetailsScope).RequestDetails;

            var userClaims = GetOutputClaims(principal, requestDetails, _userRepository);
            var outputIdentity = new ClaimsIdentity(userClaims);

            if (requestDetails.IsActAsRequest)
            {
                Tracing.Tracing.Information("Issuing act as token");
                return GetActAsClaimsIdentity(outputIdentity, requestDetails);
            }
            else
            {
                Tracing.Tracing.Information("Issuing identity token");
                return outputIdentity;
            }
        }

        public static List<Claim> GetOutputClaims(IClaimsPrincipal principal, RequestDetails requestDetails, IUserRepository userRepository)
        {
            var name = principal.FindClaims(ClaimTypes.Name).First().Value;
            var nameId = new Claim(ClaimTypes.NameIdentifier, name);

            var userClaims = new List<Claim> 
            {
                new Claim(ClaimTypes.Name, name),
                nameId,
                new Claim(ClaimTypes.AuthenticationMethod, principal.FindClaims(ClaimTypes.AuthenticationMethod).First().Value),
                AuthenticationInstantClaim.Now
            };

            userClaims.AddRange(userRepository.GetClaims(principal, requestDetails));

            return userClaims;
        }

        protected virtual IClaimsIdentity GetActAsClaimsIdentity(IClaimsIdentity clientIdentity, RequestDetails requestDetails)
        {
            var actAsSubject = requestDetails.Request.ActAs.GetSubject()[0];
            var actAsIdentity = actAsSubject.Copy();

            // find the last actor in the actAs identity
            IClaimsIdentity lastActor = actAsIdentity;
            while (lastActor.Actor != null)
            {
                lastActor = lastActor.Actor;
            }

            // set the caller's identity as the last actor in the delegation chain
            lastActor.Actor = clientIdentity;

            Tracing.Tracing.Information("ActAs client identity: " + actAsIdentity.Name);
            Tracing.Tracing.Information("ActAs actor identity : " + actAsIdentity.Actor.Name);

            // return the actAsIdentity instead of the caller's identity in this case
            return actAsIdentity;
        }

        #region FederationMessageTracing
        protected override RequestSecurityTokenResponse GetResponse(RequestSecurityToken request, SecurityTokenDescriptor tokenDescriptor)
        {
            var response = base.GetResponse(request, tokenDescriptor);

            if (GlobalConfiguration.EnableFederationMessageTracing)
            {
                Tracing.Tracing.Information(SerializeRequest(request));
                Tracing.Tracing.Information(SerializeResponse(response));
                Tracing.Tracing.Information(SerializeToken(tokenDescriptor));
            }

            return response;
        }

        private string SerializeRequest(RequestSecurityToken request)
        {
            var serializer = new WSTrust13RequestSerializer();
            var context = new WSTrustSerializationContext();
            var sb = new StringBuilder(128);

            using (var writer = XmlWriter.Create(new StringWriter(sb)))
            {
                serializer.WriteXml(request, writer, context);
                return sb.ToString();
            }
        }

        private string SerializeResponse(RequestSecurityTokenResponse response)
        {
            var serializer = new WSTrust13ResponseSerializer();
            var context = new WSTrustSerializationContext(FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlerCollectionManager);
            var sb = new StringBuilder(128);

            using (var writer = XmlWriter.Create(new StringWriter(sb)))
            {
                serializer.WriteXml(response, writer, context);
                return sb.ToString();
            }
        }

        private string SerializeToken(SecurityTokenDescriptor tokenDescriptor)
        {
            // see if token is encrypted
            var encryptedToken = tokenDescriptor.Token as EncryptedSecurityToken;
            SecurityToken token;

            if (encryptedToken != null)
            {
                // use inner token
                token = encryptedToken.Token;
            }
            else
            {
                // if not, use the token directly
                token = tokenDescriptor.Token;
            }

            var sb = new StringBuilder(128);
            FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.WriteToken(XmlWriter.Create(new StringWriter(sb)), token);

            return sb.ToString();
        }
        #endregion
    }
}