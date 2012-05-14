/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using Highway.Identity.Core.Models;

namespace Highway.Identity.Core.Repositories
{
    public interface IRelyingPartyRepository
    {
        bool TryGet(string realm, out RelyingPartyModel model);

        // management
        bool SupportsWriteAccess { get; }
        IEnumerable<RelyingPartyModel> List(int pageIndex, int pageSize);
        RelyingPartyModel Get(string id);
        void Add(RelyingPartyModel model);
        void Update(RelyingPartyModel updatedModel);
        void Delete(string id);
    }
}
