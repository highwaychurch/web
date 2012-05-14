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
using Highway.Identity.Core.Models;

namespace Highway.Identity.Core.Repositories
{
    public interface IClientCertificateRepository
    {
        // run time
        bool TryGetUserNameFromThumbprint(X509Certificate2 certificate, out string userName);

        // management
        bool SupportsWriteAccess { get; }
        IEnumerable<string> List(int pageIndex, int pageSize);
        IEnumerable<ClientCertificateModel> GetClientCertificatesForUser(string userName);
        void Add(ClientCertificateModel model);
        void Delete(ClientCertificateModel certificateModel);
    }
}
