using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.Raven.Documents;
using Highway.Identity.Core.TokenService;
using Highway.Shared.Security;
using Microsoft.IdentityModel.Claims;
using Raven.Client;

namespace Highway.Identity.Core.Repositories.Raven
{
    public class UserRepository : IUserRepository
    {
        private const string ProfileClaimPrefix = "http://id.highway.com.au/claims/profileclaims/";

        readonly IDocumentSession _session;
        readonly IClientCertificateRepository _clientCertificateRepository;

        public UserRepository(
            IDocumentSession session,
            IClientCertificateRepository clientCertificateRepository)
        {
            _session = session;
            _clientCertificateRepository = clientCertificateRepository;
        }

        public bool ValidateUser(string userName, string password)
        {
            var hashedPassword = CryptographyHelpers.HashPassword(password);
            return _session.Query<UserAccount>().Any(u => u.Username == userName && u.HashedPassword == hashedPassword);
        }

        public bool ValidateUser(X509Certificate2 clientCertificate, out string userName)
        {
            return _clientCertificateRepository.TryGetUserNameFromThumbprint(clientCertificate, out userName);
        }

        public IEnumerable<string> GetRoles(string userName, RoleTypes roleType)
        {
            if (roleType == RoleTypes.IdentityServer || roleType == RoleTypes.All)
            {
                yield return Constants.Roles.IdentityServerAdministrators;
                yield return Constants.Roles.Administrators;
            }
            else if (roleType == RoleTypes.Client)
            {
                yield return Constants.Roles.Administrators;
            }
        }

        public IEnumerable<Claim> GetClaims(IClaimsPrincipal principal, RequestDetails requestDetails)
        {
            var userName = principal.Identity.Name;
            var claims = new List<Claim>();

            // email address
            var email = "michaelnoonan@gmail.com";
            if (!string.IsNullOrEmpty(email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }

            // roles
            GetRoles(userName, RoleTypes.Client).ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            // profile claims
            claims.Add(new Claim(ProfileClaimPrefix + "fullname", "Michael Noonan"));

            return claims;
        }

        public IEnumerable<string> GetSupportedClaimTypes()
        {
            yield return ClaimTypes.Role;
            yield return ProfileClaimPrefix + "fullname";
        }

        public void Add(UserAccountModel model)
        {
            var doc = new UserAccount
                          {
                              UniqueId = Guid.NewGuid().ToString(),
                              Username = model.Username,
                              HashedPassword = model.HashedPassword,
                          };
            model.LinkedAccounts
                .Select(la =>
                        new LinkedAccount
                            {
                                ClaimedIdentifier = la.ClaimedIdentifier,
                                ProviderName = la.ProviderName,
                                ProviderUri = la.ProviderUri
                            })
                .ToList().ForEach(doc.LinkedAccounts.Add);

            model.ClientCertificates
                .Select(c =>
                        new ClientCertificate()
                            {
                                Thumbprint = c.Thumbprint,
                                Description = c.Description
                            })
                .ToList().ForEach(doc.ClientCertificates.Add);
        }
    }
}
