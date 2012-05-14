/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;

namespace Highway.Identity.Web.ViewModels.Administration
{
    public class RelyingPartiesViewModel
    {
        public List<RelyingPartyViewModel> RelyingParties { get; set; }
    }
}