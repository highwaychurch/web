using System;
using System.Collections.Generic;
using System.Linq;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.Raven.Documents;
using Raven.Client;

namespace Highway.Identity.Core.Repositories.Raven
{
    public class DelegationRepository : IDelegationRepository
    {
        readonly IDocumentSession _session;

        public DelegationRepository(IDocumentSession session)
        {
            _session = session;
        }

        public bool IsDelegationAllowed(string userName, string realm)
        {
            var record = (from entry in _session.Query<Delegation>()
                          where entry.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                                entry.Realm.Equals(realm, StringComparison.OrdinalIgnoreCase)
                          select entry).FirstOrDefault();

            return (record != null);
        }

        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<string> GetAllUsers(int pageIndex, int pageSize)
        {
            var users =
                (from user in _session.Query<Delegation>()
                 orderby user.UserName
                 select user.UserName)
                .Distinct();

            if (pageIndex != -1 && pageSize != -1)
            {
                users = users.Skip(pageIndex * pageSize).Take(pageSize);
            }

            return users.ToList();
        }

        public IEnumerable<DelegationModel> GetDelegationSettingsForUser(string userName)
        {
            var settings =
                 from record in _session.Query<Delegation>()
                 where record.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                 select record;

            return settings.ToList().ToModels();
        }

        public void Add(DelegationModel model)
        {
            var doc = new Delegation
            {
                UserName = model.UserName,
                Realm = model.Realm.AbsoluteUri,
                Description = model.Description
            };

            _session.Store(doc);
        }

        public void Delete(DelegationModel model)
        {
            var doc = _session.Load<Delegation>(model.Id);
            _session.Delete(doc);
        }
    }
}
