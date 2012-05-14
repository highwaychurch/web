/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Highway.Identity.Core.TokenService;
using Microsoft.IdentityModel.Claims;

namespace Highway.Identity.Core.Repositories
{
    public interface IUserRepository
    {
        bool ValidateUser(string userName, string password);
        bool ValidateUser(X509Certificate2 clientCertificate, out string userName);
        IEnumerable<string> GetRoles(string userName, RoleTypes roleType);
        IEnumerable<Claim> GetClaims(IClaimsPrincipal principal, RequestDetails requestDetails);
        IEnumerable<string> GetSupportedClaimTypes();
    }

    public enum RoleTypes
    {
        All,
        IdentityServer,
        Client
    }
}