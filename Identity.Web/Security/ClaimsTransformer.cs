/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Linq;
using Highway.Identity.Core.Repositories;
using Microsoft.IdentityModel.Claims;

namespace Highway.Identity.Web.Security
{
    public class ClaimsTransformer : ClaimsAuthenticationManager
    {
        public static Func<IUserRepository> UserRepositoryFactoryMethod { get; set; }

        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }

            var userRepository = UserRepositoryFactoryMethod();

            userRepository.GetRoles(incomingPrincipal.Identity.Name, RoleTypes.IdentityServer).ToList().ForEach(role =>
                incomingPrincipal.Identities[0].Claims.Add(new Claim(ClaimTypes.Role, role)));

            return incomingPrincipal;
        }
    }
}