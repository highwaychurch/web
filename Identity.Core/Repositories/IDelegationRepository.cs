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
    public interface IDelegationRepository
    {
        // run time
        bool IsDelegationAllowed(string userName, string realm);

        // management
        bool SupportsWriteAccess { get; }
        IEnumerable<string> GetAllUsers(int pageIndex, int pageSize);
        IEnumerable<DelegationModel> GetDelegationSettingsForUser(string userName);
        void Add(DelegationModel model);
        void Delete(DelegationModel model);
    }
}
