using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.Raven.Documents;
using Raven.Client;

namespace Highway.Identity.Core.Repositories.Raven
{
    public class ClientCertificateRepository : IClientCertificateRepository
    {
        readonly IDocumentSession _session;

        public ClientCertificateRepository(IDocumentSession session)
        {
            _session = session;
        }

        public bool TryGetUserNameFromThumbprint(X509Certificate2 certificate, out string userName)
        {
            userName = null;

            userName = (from mapping in _session.Query<ClientCertificate>()
                            where mapping.Thumbprint.Equals(certificate.Thumbprint, StringComparison.OrdinalIgnoreCase)
                            select mapping.UserName).FirstOrDefault();

            return (userName != null);
        }

        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<string> List(int pageIndex, int pageSize)
        {
            var users =
                (from user in _session.Query<ClientCertificate>()
                 orderby user.UserName
                 select user.UserName)
                .Distinct();

            if (pageIndex != -1 && pageSize != -1)
            {
                users = users.Skip(pageIndex * pageSize).Take(pageSize);
            }

            return users.ToList();
        }

        public IEnumerable<ClientCertificateModel> GetClientCertificatesForUser(string userName)
        {
            var certs =
                 from record in _session.Query<ClientCertificate>()
                 where record.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                 select record;

            return certs.ToList().ToModels();
        }

        public void Add(ClientCertificateModel model)
        {
            var doc = new ClientCertificate
                          {
                              UserName = model.UserName,
                              Thumbprint = model.Thumbprint,
                              Description = model.Description
                          };

            _session.Store(doc);
        }

        public void Delete(ClientCertificateModel model)
        {
            var doc = _session.Load<ClientCertificate>(model.Id);
            _session.Delete(doc);
        }
    }
}
