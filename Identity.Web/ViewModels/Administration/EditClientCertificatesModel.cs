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

namespace Highway.Identity.Web.ViewModels.Administration
{
    public class EditClientCertificatesModel
    {
        public string UserName { get; set; }
        public IEnumerable<ClientCertificateModel> ClientCertificates { get; set; }
    }
}