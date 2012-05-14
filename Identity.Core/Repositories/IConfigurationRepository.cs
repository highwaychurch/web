/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using Highway.Identity.Core.Models;

namespace Highway.Identity.Core.Repositories
{
    public interface IConfigurationRepository
    {
        GlobalConfigurationModel Configuration { get; }
        EndpointsConfigurationModel Endpoints { get; }
        CertificateModel SslCertificate { get; }
        CertificateModel SigningCertificate { get; }

        bool SupportsWriteAccess { get; }
        void UpdateConfiguration(GlobalConfigurationModel updatedConfiguration);
        void UpdateEndpoints(EndpointsConfigurationModel endpoints);
        void UpdateCertificates(string sslSubjectName, string signingSubjectName);
    }
}