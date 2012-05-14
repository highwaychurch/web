using System.Collections.Generic;
using System.Linq;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.Raven.Documents;
using Raven.Client;

namespace Highway.Identity.Core.Repositories.Raven
{
    public class RelyingPartyRepository : IRelyingPartyRepository
    {
        readonly IDocumentSession _session;

        public RelyingPartyRepository(IDocumentSession session)
        {
            _session = session;
        }

        public bool TryGet(string realm, out RelyingPartyModel model)
        {
            model = null;

            var strippedRealm = realm.StripProtocolMoniker();

            var bestMatch = (from rp in _session.Query<RelyingParty>()
                                where strippedRealm.Contains(rp.Realm)
                                orderby rp.Realm descending
                                select rp)
                            .FirstOrDefault();

            if (bestMatch != null)
            {
                model = bestMatch.ToModel();
                return true;
            }

            return false;
        }

        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<RelyingPartyModel> List(int pageIndex, int pageSize)
        {
            var rps = from e in _session.Query<RelyingParty>()
                      orderby e.Name
                      select e;

            if (pageIndex != -1 && pageSize != -1)
            {
                rps = rps.Skip(pageIndex * pageSize).Take(pageSize).OrderBy(rp => rp.Name);
            }

            return rps.ToList().ToModel();
        }

        public RelyingPartyModel Get(string id)
        {
            return _session.Load<RelyingParty>(id).ToModel();
        }

        public void Add(RelyingPartyModel model)
        {
            var doc = model.ToDocument();
            _session.Store(doc);
        }

        public void Update(RelyingPartyModel updatedModel)
        {
            var doc = _session.Load<RelyingParty>(updatedModel.Id);
            updatedModel.MergeWithDocument(doc);
        }

        public void Delete(string id)
        {
            var doc = _session.Load<RelyingParty>(id);
            _session.Delete(doc);
        }
    }
}
