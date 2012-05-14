/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Thinktecture.IdentityModel.Extensions;

namespace Highway.Identity.Web.Security
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public static Func<IConfigurationRepository> ConfigurationRepositoryFactoryMethod { get; set; }

        public override bool CheckAccess(AuthorizationContext context)
        {
            var action = context.Action.First();
            var id = context.Principal.Identities.First();

            // if application authorization request
            if (action.ClaimType.Equals(ClaimsAuthorize.ActionType))
            {
                return AuthorizeCore(action, context.Resource, context.Principal.Identity as IClaimsIdentity);
            }

            // if ws-trust issue request
            if (action.Value.Equals(WSTrust13Constants.Actions.Issue))
            {
                return AuthorizeTokenIssuance(new Collection<Claim> { new Claim(ClaimsAuthorize.ResourceType, Constants.Resources.WSTrust) }, id);
            }

            return base.CheckAccess(context);
        }

        protected virtual bool AuthorizeCore(Claim action, Collection<Claim> resource, IClaimsIdentity id)
        {
            switch (action.Value)
            {
                case Constants.Actions.Issue:
                    return AuthorizeTokenIssuance(resource, id);
                case Constants.Actions.Administration:
                    return AuthorizeAdministration(resource, id);
            }

            return false;
        }

        protected virtual bool AuthorizeTokenIssuance(Collection<Claim> resource, IClaimsIdentity id)
        {
            var configurationRepository = ConfigurationRepositoryFactoryMethod();
            if (!configurationRepository.Configuration.EnforceUsersGroupMembership)
            {
                return id.IsAuthenticated;
            }
            
            return (id.ClaimExists(ClaimTypes.Role, Constants.Roles.IdentityServerUsers));
        }

        protected virtual bool AuthorizeAdministration(Collection<Claim> resource, IClaimsIdentity id)
        {
            return (id.ClaimExists(ClaimTypes.Role, Constants.Roles.IdentityServerAdministrators));
        }
    }
}