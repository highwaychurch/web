﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.DataAnnotations;

namespace Highway.Identity.Core.Repositories.EntityFramework.EntityModel
{
    public class CertificateEntity
    {
        [Key]
        public string Name { get; set; }

        public string SubjectDistinguishedName { get; set; }
    }
}
