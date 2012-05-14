/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.EntityFramework.EntityModel;

namespace Highway.Identity.Core.Repositories.EntityFramework
{
    public class RelyingPartyRepository : IRelyingPartyRepository
    {
        public bool TryGet(string realm, out RelyingPartyModel model)
        {
            model = null;

            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var strippedRealm = realm.StripProtocolMoniker();

                var bestMatch = (from rp in entities.RelyingParties
                                 where strippedRealm.Contains(rp.Realm)
                                 orderby rp.Realm descending
                                 select rp)
                                .FirstOrDefault();

                if (bestMatch != null)
                {
                    model = bestMatch.ToDomainModel();
                    return true;
                }
            }

            return false;
        }

        #region Management
        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<RelyingPartyModel> List(int pageIndex, int pageSize)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var rps = from e in entities.RelyingParties
                          orderby e.Name
                          select e;

                if (pageIndex != -1 && pageSize != -1)
                {
                    rps = rps.Skip(pageIndex * pageSize).Take(pageSize).OrderBy(rp => rp.Name);
                }

                return rps.ToList().ToDomainModel();
            }
        }

        public RelyingPartyModel Get(string id)
        {
            var uniqueId = int.Parse(id);

            using (var entities = IdentityServerConfigurationContext.Get())
            {
                return
                    (from rp in entities.RelyingParties
                     where rp.Id == uniqueId
                     select rp)
                    .First()
                    .ToDomainModel();
            }
        }

        public void Add(RelyingPartyModel model)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                entities.RelyingParties.Add(model.ToEntity());
                entities.SaveChanges();
            }
        }

        public void Update(RelyingPartyModel updatedModel)
        {
            var rpEntity = updatedModel.ToEntity();

            using (var entities = IdentityServerConfigurationContext.Get())
            {
                entities.RelyingParties.Attach(rpEntity);
                entities.Entry(rpEntity).State = EntityState.Modified;
                entities.SaveChanges();
            }
        }

        public void Delete(string id)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var rpEntity = new RelyingPartyEntity { Id = int.Parse(id) };
                entities.RelyingParties.Attach(rpEntity);
                entities.Entry(rpEntity).State = EntityState.Deleted;
                entities.SaveChanges();
            }
        }
        #endregion
    }
}
